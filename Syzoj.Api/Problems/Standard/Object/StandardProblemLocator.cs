using System;
using Syzoj.Api.Object;

namespace Syzoj.Api.Problems.Standard.Object
{
    public sealed class StandardProblemLocator : DictionaryObjectLocator
    {
        public StandardProblemLocator(StandardProblemObjectLocator problem) : base(new Uri("object:///Syzoj.Api/problem-standard/"))
        {
            this.Add("problem", problem);
        }
    }
}