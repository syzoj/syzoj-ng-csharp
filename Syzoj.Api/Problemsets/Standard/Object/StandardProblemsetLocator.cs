using System;
using Syzoj.Api.Object;

namespace Syzoj.Api.Problemsets.Standard.Object
{
    public class StandardProblemsetLocator : DictionaryObjectLocator
    {
        public StandardProblemsetLocator(StandardProblemsetObjectLocator problemset) : base(new Uri("object:///Syzoj.Api/problemset-standard/"))
        {
            this.Add("problemset", problemset);
        }
    }
}