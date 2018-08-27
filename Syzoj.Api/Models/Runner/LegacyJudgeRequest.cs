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
        public class StandardParam
        {
            public string language { get; set; }
            public string code { get; set; }
            public int timeLimit { get; set; }
            public int memoryLimit { get; set; }
            public string fileIOInput { get; set; }
            public string fileIOOutput { get; set; }
        }
        [MessagePackObject(keyAsPropertyName: true)]
        public class Content : IMessagePackSerializationCallbackReceiver
        {
            public string taskId { get; set; }
            public string testData { get; set; }
            public LegacyProblemType type { get; set; }
            public int priority { get; set; }
            // Deserializes to System.Collections.Generic.Dictionary
            public object param;

            void IMessagePackSerializationCallbackReceiver.OnBeforeSerialize()
            {
                
            }

            void IMessagePackSerializationCallbackReceiver.OnAfterDeserialize()
            {
                switch(type)
                {
                    case LegacyProblemType.Standard:
                        param = MessagePackSerializer.Deserialize<StandardParam>(MessagePackSerializer.Serialize(param));
                        break;
                    case LegacyProblemType.AnswerSubmission:
                    case LegacyProblemType.Interaction:
                        throw new NotImplementedException();
                }
            }
        }
        public Content content { get; set; }
        public object extraData;
    }
}