using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Data;

namespace Syzoj.Api.Models
{
    [DbModel]
    public class ProblemsetSubmission
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProblemsetId { get; set; }
        public virtual Problemset Problemset { get; set; }
        public Guid ProblemId { get; set; }
        public virtual Problem Problem { get; set; }
        public virtual ProblemsetProblem ProblemsetProblem { get; set; }
        [Column(TypeName = "jsonb")]
        public string Content { get; set; }
        public byte[] Data { get; set; }

        public static void OnModelCreating(ApplicationDbContext dbContext, ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProblemsetSubmission>()
                .HasOne(s => s.ProblemsetProblem)
                .WithMany(ps => ps.Submissions)
                .HasForeignKey(s => new { s.ProblemsetId, s.ProblemId})
                .HasPrincipalKey(ps => new { ps.ProblemsetId, ps.ProblemId });
            modelBuilder.Entity<ProblemsetSubmission>()
                .HasOne(s => s.Problemset)
                .WithMany(ps => ps.Submissions)
                .HasForeignKey(s => s.ProblemsetId)
                .HasPrincipalKey(ps => ps.Id);
        }
    }
}