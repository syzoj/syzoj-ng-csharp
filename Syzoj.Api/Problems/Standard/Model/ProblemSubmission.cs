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
        public string ViewToken { get; set; }
        public string Language { get; set; }
        public string Code { get; set; }
        // 0 is not judged, 1 is waiting, 2 is completed
        public int Status { get; set; }

        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProblemSubmission>()
                .HasOne(ps => ps.Problem)
                .WithMany()
                .HasForeignKey(ps => ps.ProblemId);
        }
    }
}