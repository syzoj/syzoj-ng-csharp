using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Models;

namespace Syzoj.Api.Data
{
    public class ApplicationDbContext: DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbOptions)
            : base(dbOptions)
        {
        }

        public DbSet<DiscussionEntry> Discussions { get; set; }
        
        public DbSet<ReplyEntry> Replies { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DiscussionEntry>()
                .Property(b => b.ShowInBoard)
                .HasDefaultValue(false);
            modelBuilder.Entity<User>()
                .HasIndex(b => b.UserName)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(b => b.Email)
                .IsUnique();
        }
    }
}