using System;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Object
{
    public abstract class DbModelBase
    {
        [Key]
        public Guid Id { get; set; }
    }
}