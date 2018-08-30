using System;
using System.ComponentModel.DataAnnotations;

namespace Syzoj.Api.Models.Data
{
    // NEVER update the table manually
    public class BlobMetadata
    {
        [Key]
        public byte[] Hash { get; set; }
        public long Size { get; set; }
        public int ReferenceCount { get; set; }
        [ConcurrencyCheck]
        public DateTime TimeAccess { get; set; }
    }
}