using System;
using System.ComponentModel.DataAnnotations;
using Syzoj.Api.Data;

namespace Syzoj.Api.Problemsets.Standard.Model
{
    [DbModel]
    public class Problemset
    {
        [Key]
        public Guid Id { get; set; }
    }
}