using BirthdayApp.Common.CustomExceptions;
using BirthdayApp.Services.Interfaces;
using BirthdayApp.ViewModels;

namespace BirthdayApp.Services
{
    public class VotingService : IVotingService
    {
        private readonly ILogger<VotingService> _logger;

        public VotingService(ILogger<VotingService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<IList<ActiveVotingViewModel>> GetActiveVotingsAsync(CancellationToken cancellationToken)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error occured getting all active votings from DB. Error: {Message}", ex.Message);

                throw new DatabaseException("Could not load active votings from the database");
            }

            throw new NotImplementedException();
        }

        public Task<IList<FinishedVotingViewModel>> GetFinishedVotingsAsync(CancellationToken cancellationToken)
        {
            //reminder: should contain also users who did not vote

            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error occured getting all finished votings from DB. Error: {Message}", ex.Message);

                throw new DatabaseException("Could not load finished votings from the database");
            }

            throw new NotImplementedException();
        }

        public Task StartVotingAsync(string birthdayPersonId, CancellationToken cancellationToken)
        {
            //validate birthdayPersonId exists
            //cannot start voting for yourself
            //cannot start voting for a person with existing voting (active or finished) from the same year

            throw new NotImplementedException();
        }

        public Task EndVotingAsync(int votingId, CancellationToken cancellationToken)
        {
            //validate votingId exists
            //cannot end votings you didn't start
            //cannot end already finished voting

            throw new NotImplementedException();
        }

        public Task VoteAsync(int votingId, int presentId, CancellationToken cancellationToken)
        {
            //validate votingId exists
            //validate presentId exists
            //cannot vote for your own birthay voting
            //cannot vote more than once for currently active voting

            throw new NotImplementedException();
        }
    }
}
