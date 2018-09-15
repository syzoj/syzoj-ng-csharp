using System.Collections.Generic;

namespace Syzoj.Api
{
    public class ProblemsetManagerProvider
    {
        private static Dictionary<string, IProblemsetManager> providers = new Dictionary<string, IProblemsetManager>()
        {

        };

        public IProblemsetManager GetProblemsetManager(string Name)
        {
            return providers.GetValueOrDefault(Name);
        }
    }
}