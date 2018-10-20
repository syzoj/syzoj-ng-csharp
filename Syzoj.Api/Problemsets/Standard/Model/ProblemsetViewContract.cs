using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Data;

namespace Syzoj.Api.Problemsets.Standard.Model
{
    [DbModel]
    public class ProblemsetViewContract
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProblemsetId { get; set; }
        public Guid? ProblemContractId { get; set; }
        public string Title { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProblemsetViewContract>()
                .HasOne<Problemset>()
                .WithMany(ps => ps.ViewContracts)
                .HasForeignKey(pv => pv.ProblemsetId);
        }
    }
}