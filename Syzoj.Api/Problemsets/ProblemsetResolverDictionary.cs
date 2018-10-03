using System.Collections.Generic;

namespace Syzoj.Api.Problemsets
{
    public class ProblemsetResolverDictionary
    {
        private IDictionary<string, IProblemsetResolverProvider> providers;
        public ProblemsetResolverDictionary(IEnumerable<IProblemsetResolverProvider> providers)
        {
            this.providers = new Dictionary<string, IProblemsetResolverProvider>();
            foreach(var p in providers)
            {
                this.providers.Add(p.ProblemsetType, p);
            }
        }

        public IProblemsetResolverProvider GetProvider(string ProblemsetType)
        {
            return providers[ProblemsetType];
        }
    }
}