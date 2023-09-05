using BirthdayApp.ViewModels;

namespace BirthdayApp.Services.Interfaces
{
    public interface IVotingService
    {
        Task<IList<ActiveVotingViewModel>> GetActiveVotingsAsync(CancellationToken cancellationToken);

        Task<IList<FinishedVotingViewModel>> GetFinishedVotingsAsync(CancellationToken cancellationToken);

        Task StartVotingAsync(string birthdayPersonId, string currentUserId,  CancellationToken cancellationToken);

        Task EndVotingAsync(int votingId, string currentUserId, CancellationToken cancellationToken);

        Task VoteAsync(int votingId, int presentId, string currentUserId, CancellationToken cancellationToken);
    }
}
