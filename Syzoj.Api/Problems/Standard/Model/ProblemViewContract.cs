using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Data;

namespace Syzoj.Api.Problems.Standard.Model
{
    [DbModel]
    public class ProblemViewContract
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProblemId { get; set; }
        public Guid ProblemsetContractId { get; set; }
        public bool RequiresNotification { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProblemViewContract>()
                .HasOne<Problem>()
                .WithMany(p => p.ViewContracts)
                .HasForeignKey(pv => pv.ProblemId);
            modelBuilder.Entity<ProblemViewContract>()
                .HasIndex(pv => new { pv.ProblemId, pv.RequiresNotification });
        }
    }
}