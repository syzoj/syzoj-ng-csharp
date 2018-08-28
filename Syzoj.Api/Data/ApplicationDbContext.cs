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
        }
    }
}