namespace BirthdayApp.Data.Models
{
    public class Voting
    {
        public int Id { get; set; }

        public string BirthdayPersonId { get; set; }
        public virtual ApplicationUser BirthdayPerson { get; set; }

        public string InitiatorId { get; set; }
        public virtual ApplicationUser Initiator { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<UserVote> PeopleVoted { get; set; } = new HashSet<UserVote>();
    }
}
