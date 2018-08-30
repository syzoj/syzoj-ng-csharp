using System;
using System.Collections.Generic;
using MessagePack;
using Syzoj.Api.Utils;

namespace Syzoj.Api.Models.Runner
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LegacyJudgeRequest
    {
        [MessagePackObject(keyAsPropertyName: true)]
        public class Content
        {
            public string taskId { get; set; }
            public string testData { get; set; }
            public LegacyProblemType type { get; set; }
            public int priority { get; set; }
            // Deserializes to System.Collections.Generic.Dictionary
            public object param;
        }
        public Content content { get; set; }
        public object extraData;
    }
}