using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Data;

namespace Syzoj.Api.Models
{
    [DbModel]
    public class ProblemsetProblem
    {
        public Guid ProblemsetId { get; set; }
        public virtual Problemset Problemset { get; set; }
        public Guid ProblemId { get; set; }
        public virtual Problem Problem { get; set; }
        public virtual ICollection<ProblemsetSubmission> Submissions { get; set; }
        [MinLength(1)]
        [MaxLength(128)]
        [Required]
        public string ProblemsetProblemId { get; set; }
        public string Title { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProblemsetProblem>()
                .ToTable("ProblemsetProblems")
                .HasKey(psp => new { psp.ProblemsetId, psp.ProblemId });
            modelBuilder.Entity<ProblemsetProblem>()
                .HasIndex(psp => new { psp.ProblemsetId, psp.ProblemsetProblemId })
                .IsUnique();
            modelBuilder.Entity<ProblemsetProblem>()
                .HasOne(psp => psp.Problem)
                .WithMany()
                .HasForeignKey(psp => psp.ProblemId);
            modelBuilder.Entity<ProblemsetProblem>()
                .HasOne(psp => psp.Problemset)
                .WithMany(ps => ps.ProblemsetProblems)
                .HasForeignKey(psp => psp.ProblemsetId);
            modelBuilder.Entity<ProblemsetProblem>()
                .ForNpgsqlUseXminAsConcurrencyToken();
        }
    }
}