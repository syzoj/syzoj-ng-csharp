using System;
using System.Collections.Generic;
using System.Linq;

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
            
        };
        private static Dictionary<string, Type> parsers = new Dictionary<string, Type>()
        {

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