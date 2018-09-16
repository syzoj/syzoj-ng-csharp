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
        private static IEnumerable<ValueTuple<string, Type>> defaultParsers = new ValueTuple<string, Type>[]
        {
            ("syzoj-legacy", typeof(LegacySyzojProblemParser))
        };
        private static Dictionary<string, Type> parsers = new Dictionary<string, Type>()
        {
            { "syzoj-legacy", typeof(LegacySyzojProblemParser) },
        };

        public IEnumerable<ValueTuple<string, IAsyncProblemParser>> GetDefaultParsers()
        {
            return defaultParsers.Select(parser => (parser.Item1, (IAsyncProblemParser) provider.GetService(parser.Item2))).AsEnumerable();
        }

        public IAsyncProblemParser GetParser(string name)
        {
            if(name == null)
                throw new NotImplementedException();
            return (IAsyncProblemParser) provider.GetService(parsers.GetValueOrDefault(name));
        }
    }
}