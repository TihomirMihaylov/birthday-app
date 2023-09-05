using BirthdayApp.Common.CustomExceptions;
using BirthdayApp.Data.Models;
using BirthdayApp.Data.Repositories;
using BirthdayApp.Services.Interfaces;
using BirthdayApp.ViewModels;

namespace BirthdayApp.Services
{
    public class VotingService : IVotingService
    {
        private readonly ILogger<VotingService> _logger;
        private readonly IRepository<Voting> _votingRepository;
        private readonly IRepository<UserVote> _userVotesRepository;
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IRepository<Present> _presentRepository;

        public VotingService(
            ILogger<VotingService> logger,
            IRepository<Voting> votingRepository,
            IRepository<UserVote> userVotesRepository,
            IRepository<ApplicationUser> userRepository,
            IRepository<Present> presentRepository
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _votingRepository = votingRepository ?? throw new ArgumentNullException(nameof(votingRepository));
            _userVotesRepository = userVotesRepository ?? throw new ArgumentNullException(nameof(userVotesRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _presentRepository = presentRepository ?? throw new ArgumentNullException(nameof(presentRepository));
        }

        public async Task<IList<ActiveVotingViewModel>> GetActiveVotingsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var allVotings = await _votingRepository.AllAsNoTrackingAsync(cancellationToken);
                return allVotings
                    .Where(x => x.IsActive)
                    .Select(x => new ActiveVotingViewModel()
                {
                    Id = x.Id,
                    BirthdayPersonId = x.BirthdayPerson.Id,
                    BirthdayPersonFirstName = x.BirthdayPerson.FirstName,
                    BirthdayPersonLastName = x.BirthdayPerson.LastName,
                    BirthdayPersonBirthday = x.BirthdayPerson.Birthday,
                    InitiatorId = x.InitiatorId,
                    InitiatorFirstName = x.Initiator.FirstName,
                    InitiatorLastName = x.Initiator.LastName
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error occured getting all active votings from DB. Error: {Message}", ex.Message);

                throw new DatabaseException("Could not load active votings from the database");
            }
        }

        public async Task<IList<FinishedVotingViewModel>> GetFinishedVotingsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var allVotings = await _votingRepository.AllAsNoTrackingAsync(cancellationToken);
                var allUserVotes = await _userVotesRepository.AllAsNoTrackingAsync(cancellationToken);

                var allFinishedMapped = allVotings
                    .Where(v => v.IsActive)
                    .Select(v => new FinishedVotingViewModel()
                    {
                        Id = v.Id,
                        BirthdayPersonId = v.BirthdayPerson.Id,
                        BirthdayPersonFirstName = v.BirthdayPerson.FirstName,
                        BirthdayPersonLastName = v.BirthdayPerson.LastName,
                        BirthdayPersonBirthday = v.BirthdayPerson.Birthday,
                        InitiatorId = v.InitiatorId,
                        InitiatorFirstName = v.Initiator.FirstName,
                        InitiatorLastName = v.Initiator.LastName,
                        UserVotes = allUserVotes
                        .Where(uv => uv.VotingId == v.Id)
                        .Select(uv => new UserVoteViewModel()
                        {
                            UserId = uv.UserId,
                            FirstName = uv.User.FirstName,
                            LastName = uv.User.LastName,
                            PresentName = uv.Present.Name
                        }).ToList()
                    }).ToList();

                //The response should contain also users who did not vote
                var allUsers = await _userRepository.AllAsNoTrackingAsync(cancellationToken);
                foreach (var finishedVoting in allFinishedMapped)
                {
                    var peopleNotVoted = new List<UserVoteViewModel>();
                    foreach (var user in allUsers)
                    {
                        if (!finishedVoting.UserVotes.Select(x => x.UserId).Contains(user.Id))
                        {
                            peopleNotVoted.Add(new UserVoteViewModel()
                            {
                                UserId = user.Id,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                PresentName = "not voted"
                            });
                        }
                    }

                    foreach (var personNotVoted in peopleNotVoted)
                    {
                        finishedVoting.UserVotes.Add(personNotVoted);
                    }
                }

                return allFinishedMapped;
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error occured getting all finished votings from DB. Error: {Message}", ex.Message);

                throw new DatabaseException("Could not load finished votings from the database");
            }
        }

        public async Task StartVotingAsync(
            string birthdayPersonId,
            string currentUserId,
            CancellationToken cancellationToken
        )
        {
            await ValidateStartVotingRequestAsync(birthdayPersonId, currentUserId, cancellationToken);

            var newVoting = new Voting()
            {
                BirthdayPersonId = birthdayPersonId,
                InitiatorId = currentUserId,
                IsActive = true
            };

            await _votingRepository.AddAsync(newVoting, cancellationToken);
            await _votingRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task EndVotingAsync(
            int votingId,
            string currentUserId,
            CancellationToken cancellationToken
        )
        {
            var currentVoting = await _votingRepository.GetByIdAsync(cancellationToken, votingId);
            ValidateEndVotingRequest(votingId, currentVoting, currentUserId);

            currentVoting.IsActive = false;
            _votingRepository.Update(currentVoting);
            await _votingRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task VoteAsync(
            int votingId,
            int presentId,
            string currentUserId,
            CancellationToken cancellationToken
        )
        {
            await ValidateVoteRequestAsync(votingId, presentId, currentUserId, cancellationToken);

            var newUserVote = new UserVote()
            {
                UserId = currentUserId,
                VotingId = votingId,
                PresentId = presentId
            };

            await _userVotesRepository.AddAsync(newUserVote, cancellationToken);
            await _userVotesRepository.SaveChangesAsync(cancellationToken);
        }

        private async Task ValidateStartVotingRequestAsync(
            string birthdayPersonId,
            string currentUserId,
            CancellationToken cancellationToken
        )
        {
            if (birthdayPersonId == currentUserId)
            {
                throw new ValidationException("Cannot start voting for your own birthday");
            }

            var birthdayPerson = await _userRepository.GetByIdAsync(cancellationToken, birthdayPersonId);
            if (birthdayPerson == null)
            {
                throw new EntityNotFoundException($"Person with id {birthdayPersonId} not found");
            }

            var allVotings = await _votingRepository.AllAsNoTrackingAsync(cancellationToken);
            var votingsForThisPerson = allVotings.Where(x => x.BirthdayPersonId == birthdayPersonId);
            if (votingsForThisPerson.Any(x => x.IsActive))
            {
                throw new ValidationException("This person already has an active voting");
            }
            if (votingsForThisPerson.Any(x => x.BirthdayPerson.Birthday.Year == DateTime.UtcNow.Year))
            {
                throw new ValidationException("Cannot start a new voting for this person till the end of the year");
            }
        }

        private static void ValidateEndVotingRequest(
            int votingId,
            Voting currentVoting,
            string currentUserId
        )
        {
            if (currentVoting == null)
            {
                throw new EntityNotFoundException($"Voting with id {votingId} not found");
            }
            if (currentVoting.InitiatorId != currentUserId)
            {
                throw new ValidationException("Cannot end a voting started by someone else");
            }
            if (!currentVoting.IsActive)
            {
                throw new ValidationException("Cannot vote in a finished voting");
            }

            if (!currentVoting.IsActive)
            {
                throw new ValidationException("The voting is already finished");
            }
        }

        private async Task ValidateVoteRequestAsync(
            int votingId,
            int presentId,
            string currentUserId,
            CancellationToken cancellationToken
        )
        {
            var currentVoting = await _votingRepository.GetByIdAsync(cancellationToken, votingId);
            if (currentVoting == null)
            {
                throw new EntityNotFoundException($"Voting with id {votingId} not found");
            }
            if (currentVoting.BirthdayPersonId == currentUserId)
            {
                throw new ValidationException("Voting for your own birthday is not allowed");
            }

            var present = await _presentRepository.GetByIdAsync(cancellationToken, presentId);
            if (present == null)
            {
                throw new EntityNotFoundException($"Present with id {presentId} not found");
            }

            var allUserVotes = await _userVotesRepository.AllAsNoTrackingAsync(cancellationToken);
            if (allUserVotes.Any(x => x.VotingId == votingId && x.UserId == currentUserId))
            {
                throw new ValidationException("You cannot vote twice");
            }
        }
    }
}
