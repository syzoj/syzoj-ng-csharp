using System;
using System.Collections.Generic;
using MessagePack;

namespace Syzoj.Api.Problems.Standard.Model
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class StandardTestData
    {
        public IDictionary<Guid, StandardTestCase> TestCases { get; set; }
        public IList<Subtask> Subtasks { get; set; }
    }
}