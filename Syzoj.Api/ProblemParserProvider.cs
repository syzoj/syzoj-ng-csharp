using System;
using System.Collections.Generic;
using System.Linq;
using Syzoj.Api.Services;

namespace Syzoj.Api
{
    public class ProblemParserProvider
    {
        private readonly IServiceProvider provider;
        public ProblemParserProvider(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public ProblemParserProvider()
        {
        }

        private static Dictionary<string, Type> parsers = new Dictionary<string, Type>()
        {
            { "default", typeof(DefaultProblemParser) },
        };

        public IAsyncProblemParser GetParser(string name)
        {
            if(name == null)
                throw new ArgumentNullException();
            var type = parsers.GetValueOrDefault(name);
            if(type == null)
                return null;
            return (IAsyncProblemParser) provider.GetService(type);
        }
    }
}