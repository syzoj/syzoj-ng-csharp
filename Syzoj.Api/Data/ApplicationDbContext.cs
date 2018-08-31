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
        public DbSet<ProblemSubmission> ProblemSubmissions { get; set; }
        public DbSet<BlobMetadata> BlobMetadata { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasIndex(b => b.UserName)
                .IsUnique()
                .HasName("IX_Users_UserName");
            modelBuilder.Entity<User>()
                .HasIndex(b => b.Email)
                .IsUnique()
                .HasName("IX_Users_Email");
            
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
                .HasKey(psp => new { psp.ProblemSetId, psp.ProblemSetProblemId })
                .HasName("PK_ProblemSetProblems");
            modelBuilder.Entity<ProblemSetProblem>()
                .HasIndex(psp => new{ psp.ProblemSetId, psp.ProblemId })
                .IsUnique()
                .HasName("IX_ProblemSetProblems_ProblemSetId_ProblemId"); // TODO: Is this necessary?
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
            modelBuilder.Entity<ProblemSetProblem>()
                .HasIndex(psp => psp.AcceptsToSubmissions);
            modelBuilder.Entity<ProblemSetProblem>()
                .ForNpgsqlUseXminAsConcurrencyToken();
            
            modelBuilder.Entity<ProblemSubmission>()
                .HasOne(ps => ps.Problem)
                .WithMany(p => p.ProblemSubmissions)
                .HasForeignKey(ps => ps.ProblemId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ProblemSubmission>()
                .HasOne(ps => ps.ProblemSet)
                .WithMany(ps => ps.ProblemSubmissions)
                .HasForeignKey(ps => ps.ProblemSetId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ProblemSubmission>()
                .HasOne(ps => ps.ProblemSetProblem)
                .WithMany(psp => psp.ProblemSubmissions)
                .HasForeignKey(ps => new { ps.ProblemSetId, ps.ProblemId })
                .HasPrincipalKey(psp => new { psp.ProblemSetId, psp.ProblemId });
            modelBuilder.Entity<ProblemSubmission>()
                .HasOne(ps => ps.User)
                .WithMany(u => u.ProblemSubmissions)
                .HasForeignKey(ps => ps.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ProblemSubmission>()
                .ForNpgsqlUseXminAsConcurrencyToken();
            
            modelBuilder.Entity<BlobMetadata>()
                .HasIndex(bm => new { bm.ReferenceCount, bm.TimeAccess });

            modelBuilder.Entity<Forum>().HasData(
                new Forum() { Id = 1, Info = "Announcements" },
                new Forum() { Id = 2, Info = "Default forum" },
                new Forum() { Id = 3, Info = "Problem forum" });
            modelBuilder.Entity<ProblemSet>().HasData(
                new ProblemSet() { Id = 1, Info = "Default problem set", Type = ProblemSetType.Default });
        }
    }
}