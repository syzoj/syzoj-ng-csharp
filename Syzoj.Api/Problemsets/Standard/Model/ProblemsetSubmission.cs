using System;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Data;
using Syzoj.Api.Object;

namespace Syzoj.Api.Problemsets.Standard.Model
{
    [DbModel]
    public class ProblemsetSubmission
    {
        public Guid Id { get; set; }
        public virtual Problemset Problemset { get; set; }
        public Guid ProblemsetId { get; set; }
        public Guid SubmissionContractId { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProblemsetSubmission>()
                .HasOne(ps => ps.Problemset)
                .WithMany()
                .HasForeignKey(ps => ps.ProblemsetId);
        }
    }
}