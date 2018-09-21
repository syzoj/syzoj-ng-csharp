using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Models;

namespace Syzoj.Api.Data
{
    public class ApplicationDbContext : IdentityUserContext<ApplicationUser>
    {
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Problemset> Problemsets { get; set; }
        public DbSet<ProblemsetProblem> ProblemsetProblems { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            byte[] nilData = MessagePack.MessagePackSerializer.Serialize(new MessagePack.Nil());

            modelBuilder.Entity<ProblemsetProblem>()
                .HasKey(psp => new { psp.ProblemsetId, psp.ProblemId });
            modelBuilder.Entity<ProblemsetProblem>()
                .HasIndex(psp => new { psp.ProblemsetId, psp.ProblemsetProblemId })
                .IsUnique();
            modelBuilder.Entity<ProblemsetProblem>()
                .HasOne(psp => psp.Problem)
                .WithMany()
                .HasForeignKey(psp => psp.ProblemId)
                .HasPrincipalKey(p => p.Id);
            modelBuilder.Entity<ProblemsetProblem>()
                .HasOne(psp => psp.Problemset)
                .WithMany(ps => ps.ProblemsetProblems)
                .HasForeignKey(psp => psp.ProblemsetId)
                .HasPrincipalKey(ps => ps.Id);
            
            modelBuilder.Entity<Submission>()
                .HasOne(s => s.ProblemsetProblem)
                .WithMany(ps => ps.Submissions)
                .HasForeignKey(s => new { s.ProblemsetId, s.ProblemId})
                .HasPrincipalKey(ps => new { ps.ProblemsetId, ps.ProblemId });
            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Problemset)
                .WithMany(ps => ps.Submissions)
                .HasForeignKey(s => s.ProblemsetId)
                .HasPrincipalKey(ps => ps.Id);
        }
    }
}