using System;
using System.Collections.Generic;
using Syzoj.Api.Data;
using Syzoj.Api.Object;

namespace Syzoj.Api.Problems.Standard.Model
{
    [DbModel]
    public class Problem : DbModelBase
    {
        public int Status { get; set; }
        public byte[] _Statement { get; set; }
        public byte[] _TestData { get; set; }
    }
}