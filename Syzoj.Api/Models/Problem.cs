using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Syzoj.Api.Data;

namespace Syzoj.Api.Models
{
    [DbModel]
    public class Problem
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string ProblemType { get; set; }
        [Column(TypeName = "jsonb")]
        public string Content { get; set; }
        public byte[] Data { get; set; }

        public static void OnModelCreating(ApplicationDbContext dbContext, ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Problem>()
                .ForNpgsqlUseXminAsConcurrencyToken();
        }
    }
}