namespace BirthdayApp.Data.Models
{
    public class Present //will be inserted via script
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public virtual ICollection<UserVote> UserVotes { get; set; } = new HashSet<UserVote>();
    }
}
