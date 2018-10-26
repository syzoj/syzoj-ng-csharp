using System.Collections.Generic;
using Syzoj.Api.Data;
using Syzoj.Api.Object;

namespace Syzoj.Api.Problemsets.Standard.Model
{
    [DbModel]
    public class Problemset : DbModelBase
    {
        public virtual IEnumerable<ProblemsetProblem> Problems { get; set; }
    }
}