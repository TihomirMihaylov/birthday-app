using Microsoft.AspNetCore.Identity;

namespace BirthdayApp.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime Birthday { get; set; }

        public virtual ICollection<Voting> VotingsForMyBirthdays { get; set; } = new HashSet<Voting>();

        public virtual ICollection<Voting> VotingsInitiatedByMe { get; set; } = new HashSet<Voting>();

        public virtual ICollection<UserVote> MyVotes { get; set; } = new HashSet<UserVote>();
    }
}
