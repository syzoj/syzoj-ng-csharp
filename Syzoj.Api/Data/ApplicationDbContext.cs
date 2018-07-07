using Microsoft.AspNetCore;
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

        public DbSet<User> Users { get; set; }
        
        public DbSet<DiscussionEntry> Discussions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DiscussionEntry>()
                .Property(b => b.ShowInBoard)
                .HasDefaultValue(false);
        }
    }
}