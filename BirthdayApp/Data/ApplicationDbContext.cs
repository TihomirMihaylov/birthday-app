using BirthdayApp.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BirthdayApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public override DbSet<ApplicationUser> Users { get; set; }

        public DbSet<Voting> Voting { get; set; }

        public DbSet<UserVote> UserVotes { get; set; }

        public DbSet<Present> Presents { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Needed for Identity models configuration
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.VotingsForMyBirthdays)
                .WithOne(v => v.BirthdayPerson)
                .HasForeignKey(e => e.BirthdayPersonId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.VotingsInitiatedByMe)
                .WithOne(v => v.Initiator)
                .HasForeignKey(e => e.InitiatorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.MyVotes)
                .WithOne(uv => uv.User)
                .HasForeignKey(uv => uv.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Voting>()
                .HasMany(v => v.PeopleVoted)
                .WithOne(uv => uv.Voting)
                .HasForeignKey(uv => uv.VotingId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Present>()
                .HasMany(p => p.UserVotes)
                .WithOne(uv => uv.Present)
                .HasForeignKey(uv => uv.PresentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserVote>()
                .HasOne(uv => uv.Voting)
                .WithMany(v => v.PeopleVoted)
                .HasForeignKey(uv => uv.VotingId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserVote>()
                 .HasKey(uv => new { uv.UserId, uv.VotingId });
        }
    }
}