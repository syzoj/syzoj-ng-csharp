using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Models;

namespace Syzoj.Api.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbOptions)
            : base(dbOptions)
        {
        }

        public DbSet<DiscussionEntry> Discussions { get; set; }
        
        public DbSet<ReplyEntry> Replies { get; set; }
        
        public DbSet<LoginSession> LoginSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DiscussionEntry>()
                .Property(b => b.ShowInBoard)
                .HasDefaultValue(false);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Sessions)
                .WithOne(s => s.User)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}