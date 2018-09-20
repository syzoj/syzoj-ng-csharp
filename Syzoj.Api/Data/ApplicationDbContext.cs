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

            modelBuilder.Entity<ProblemsetProblem>()
                .HasKey(psp => new { psp.ProblemsetId, psp.ProblemId });
            modelBuilder.Entity<ProblemsetProblem>()
                .HasAlternateKey(psp => new { psp.ProblemsetId, psp.ProblemsetProblemId });
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
            
            byte[] nilData = MessagePack.MessagePackSerializer.Serialize(new MessagePack.Nil());
            var defaultProblemset = new Problemset() { Id = Guid.Parse("057f55f3-9c14-4811-a213-c4b80e03174c"), Type = "debug" };
            modelBuilder.Entity<Problemset>()
                .HasData(defaultProblemset);
            var defaultProblem = new Problem() { Id = Guid.Parse("057f55f3-9c14-4811-a213-c4b80e03174c"), ProblemType = null, Path = "/data/problem/1/", Title = "Test problem", Statement = nilData, IsSubmittable = false };
            modelBuilder.Entity<Problem>()
                .HasData(defaultProblem);
            var defaultProblemsetProblem = new ProblemsetProblem() { ProblemsetId = defaultProblemset.Id, ProblemId = defaultProblem.Id, ProblemsetProblemId = "debug" };
            modelBuilder.Entity<ProblemsetProblem>()
                .HasData(defaultProblemsetProblem);
            var defaultSubmission = new Submission() { Id = Guid.Parse("057f55f3-9c14-4811-a213-c4b80e03174c"), ProblemId = defaultProblem.Id, ProblemsetId = defaultProblemset.Id, Summary = nilData, Content = nilData };
            modelBuilder.Entity<Submission>()
                .HasData(defaultSubmission);
        }
    }
}