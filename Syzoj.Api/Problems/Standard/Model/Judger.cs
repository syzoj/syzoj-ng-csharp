using System;
using System.ComponentModel.DataAnnotations;
using Syzoj.Api.Data;

namespace Syzoj.Api.Problems.Standard.Model
{
    [DbModel]
    public class Judger
    {
        [Key]
        public Guid Id { get; set; }
        public string Token { get; set; }
    }
}