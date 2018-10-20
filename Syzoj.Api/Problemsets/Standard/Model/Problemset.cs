using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Syzoj.Api.Data;
using Syzoj.Api.Object;

namespace Syzoj.Api.Problemsets.Standard.Model
{
    [DbModel]
    public class Problemset : DbModelBase
    {
        public IEnumerable<ProblemsetViewContract> ViewContracts { get; set; }
    }
}