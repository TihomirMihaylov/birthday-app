namespace BirthdayApp.Data.Models
{
    public class UserVote
    {
        public int VotingId { get; set; }
        public Voting Voting { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int PresentId { get; set; }
        public Present Present { get; set; }
    }
}
