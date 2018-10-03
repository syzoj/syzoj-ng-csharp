using System.Collections.Generic;

namespace Syzoj.Api.Problems
{
    public class ProblemResolverDictionary
    {
        private IDictionary<string, IProblemResolverProvider> providers;
        public ProblemResolverDictionary(IEnumerable<IProblemResolverProvider> providers)
        {
            this.providers = new Dictionary<string, IProblemResolverProvider>();
            foreach(var p in providers)
            {
                this.providers.Add(p.ProblemType, p);
            }
        }

        public IProblemResolverProvider GetProvider(string ProblemType)
        {
            return providers[ProblemType];
        }
    }
}