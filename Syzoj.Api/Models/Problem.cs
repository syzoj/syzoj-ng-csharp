using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Syzoj.Api.Models
{
    public class Problem
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string ProblemType { get; set; }
        [Column(TypeName = "jsonb")]
        public string Content { get; set; }
        public byte[] Data { get; set; }
    }
}