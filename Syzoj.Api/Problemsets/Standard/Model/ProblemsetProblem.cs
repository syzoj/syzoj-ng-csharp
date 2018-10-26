using System;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Data;

namespace Syzoj.Api.Problemsets.Standard.Model
{
    [DbModel]
    public class ProblemsetProblem
    {
        public Guid Id { get; set; }
        public Guid ProblemsetId { get; set; }
        public Guid ProblemContractId { get; set; }
        public virtual Problemset Problemset { get; set; }
        public string Identifier { get; set; }
        public string Title { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProblemsetProblem>()
                .HasOne(p => p.Problemset)
                .WithMany(ps => ps.Problems)
                .HasForeignKey(p => p.ProblemsetId);
            modelBuilder.Entity<ProblemsetProblem>()
                .HasIndex(ps => new { ps.ProblemsetId, ps.Identifier })
                .IsUnique();
        }
    }
}