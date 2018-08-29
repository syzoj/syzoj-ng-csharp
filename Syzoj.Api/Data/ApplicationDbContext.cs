using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Models.Data;

namespace Syzoj.Api.Data
{
    public class ApplicationDbContext: DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbOptions)
            : base(dbOptions)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<ForumDiscussion> ForumDiscussions { get; set; }
        public DbSet<DiscussionEntry> Discussions { get; set; }
        public DbSet<DiscussionReplyEntry> DiscussionReplies { get; set; }
        public DbSet<ProblemSet> ProblemSets { get; set; }
        public DbSet<ProblemSetProblem> ProblemSetProblems { get; set; }
        public DbSet<Problem> Problems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasIndex(b => b.UserName)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(b => b.Email)
                .IsUnique();
            
            modelBuilder.Entity<ForumDiscussion>()
                .HasKey(fd => new { fd.ForumId, fd.DiscussionEntryId });
            modelBuilder.Entity<ForumDiscussion>()
                .HasOne(fd => fd.Forum)
                .WithMany(f => f.DiscussionEntries)
                .HasForeignKey(fd => fd.ForumId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ForumDiscussion>()
                .HasOne(fd => fd.DiscussionEntry)
                .WithMany(d => d.Forums)
                .HasForeignKey(fd => fd.DiscussionEntryId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ForumDiscussion>()
                .HasIndex(fd => new { fd.ForumId, fd.TimeLastReply });
            
            modelBuilder.Entity<ProblemSetProblem>()
                .HasKey(psp => new { psp.ProblemSetId, psp.ProblemSetProblemId });
            modelBuilder.Entity<ProblemSetProblem>()
                .HasIndex(psp => new{ psp.ProblemSetId, psp.ProblemId })
                .IsUnique(); // TODO: Is this necessary?
            modelBuilder.Entity<ProblemSetProblem>()
                .HasOne(psp => psp.ProblemSet)
                .WithMany(ps => ps.ProblemSetProblem)
                .HasForeignKey(psp => psp.ProblemSetId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProblemSetProblem>()
                .HasOne(psp => psp.Problem)
                .WithMany(p => p.ProblemSets)
                .HasForeignKey(psp => psp.ProblemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Forum>().HasData(
                new Forum() { Id = 1, Info = "Announcements" },
                new Forum() { Id = 2, Info = "Default forum" },
                new Forum() { Id = 3, Info = "Problem forum" });
            modelBuilder.Entity<ProblemSet>().HasData(
                new ProblemSet() { Id = 1, Info = "Default problem set" });
        }
    }
}