using System;
using System.Collections.Generic;

namespace Syzoj.Api
{
    public class ProblemParserProvider
    {
        private static IEnumerable<ValueTuple<string, IProblemParser>> defaultParsers = new ValueTuple<string, IProblemParser>[]
        {
            
        };
        private static Dictionary<string, IProblemParser> parsers = new Dictionary<string, IProblemParser>()
        {

        };

        public IEnumerable<ValueTuple<string, IProblemParser>> GetDefaultParsers()
        {
            return defaultParsers;
        }

        public IProblemParser GetParser(string name)
        {
            return parsers.GetValueOrDefault(name);
        }
    }
}