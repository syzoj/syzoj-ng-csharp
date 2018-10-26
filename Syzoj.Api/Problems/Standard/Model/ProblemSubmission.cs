using System;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Data;
using Syzoj.Api.Object;

namespace Syzoj.Api.Problems.Standard.Model
{
    [DbModel]
    public class ProblemSubmission : DbModelBase
    {
        public Guid ProblemId { get; set; }
        public virtual Problem Problem { get; set; }
        public string Token { get; set; }
        public string ViewToken { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProblemSubmission>()
                .HasOne(ps => ps.Problem)
                .WithMany()
                .HasForeignKey(ps => ps.ProblemId);

            modelBuilder.Entity<ProblemSubmission>()
                .HasIndex(ps => ps.Token)
                .IsUnique();
        }
    }
}