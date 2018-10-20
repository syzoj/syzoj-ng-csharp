using System;
using System.Collections.Generic;
using Syzoj.Api.Data;

namespace Syzoj.Api.Problems.Standard.Model
{
    [DbModel]
    public class Problem
    {
        public Guid Id { get; set; }
        public byte[] _Statement { get; set; }
        public virtual ICollection<ProblemViewContract> ViewContracts { get; set; }
    }
}