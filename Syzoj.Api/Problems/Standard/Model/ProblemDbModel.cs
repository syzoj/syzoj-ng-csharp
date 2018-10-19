using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Data;

namespace Syzoj.Api.Problems.Standard.Model
{
    [DbModel]
    public class ProblemDbModel
    {
        [Key]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Tags { get; set; }
        public byte[] JudgingSettings { get; set; }
        
        public static void OnModelCreating(ApplicationDbContext dbContext, ModelBuilder modelBuilder)
        {

        }
    }
}