using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Syzoj.Api.Data;

namespace Syzoj.Api.Object
{
    [DbModel]
    public class ObjectDbModel
    {
        [Key]
        public Guid Id { get; set; }
        public string Type { get; set; }
        [Column(TypeName = "jsonb")]
        public string Info { get; set; }
    }
}