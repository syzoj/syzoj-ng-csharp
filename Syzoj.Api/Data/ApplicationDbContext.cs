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
            
            modelBuilder.Entity<Forum>().HasData(
                new Forum() {Id = 1, Info = "Announcements"},
                new Forum() {Id = 2, Info = "Default forum"},
                new Forum() {Id = 3, Info = "Problem forum"});
        }
    }
}