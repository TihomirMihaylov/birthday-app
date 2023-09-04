using BirthdayApp.ViewModels;

namespace BirthdayApp.Services.Interfaces
{
    public interface IVotingService
    {
        Task<IList<ActiveVotingViewModel>> GetActiveVotingsAsync(CancellationToken cancellationToken);

        Task<IList<FinishedVotingViewModel>> GetFinishedVotingsAsync(CancellationToken cancellationToken);

        Task StartVotingAsync(string birthdayPersonId, CancellationToken cancellationToken);

        Task EndVotingAsync(int votingId, CancellationToken cancellationToken);

        Task VoteAsync(int votingId, int presentId, CancellationToken cancellationToken);
    }
}
