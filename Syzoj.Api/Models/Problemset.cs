using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Data;

namespace Syzoj.Api.Models
{
    [DbModel]
    public class Problemset
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Type { get; set; }
        public virtual ICollection<ProblemsetProblem> ProblemsetProblems { get; set; }
        public virtual ICollection<ProblemsetSubmission> Submissions { get; set; }

        public static void OnModelCreating(ApplicationDbContext dbContext, ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Problemset>()
                .ToTable("Problemsets")
                .ForNpgsqlUseXminAsConcurrencyToken();
        }
    }
}