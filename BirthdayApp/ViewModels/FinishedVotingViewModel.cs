namespace BirthdayApp.ViewModels
{
    public class FinishedVotingViewModel : ActiveVotingViewModel
    {
        public IList<UserVoteViewModel> UserVotes { get; set; }
    }
}
