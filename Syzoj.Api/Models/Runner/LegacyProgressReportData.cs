using System.Collections.Generic;
using System.IO;
using MessagePack;
using Syzoj.Api.Utils;

namespace Syzoj.Api.Models.Runner
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LegacyProgressReportData : IMessagePackSerializationCallbackReceiver
    {
        public string taskId { get; set; }
        public LegacyProgressReportType type { get; set; }
        public object progress;

        void IMessagePackSerializationCallbackReceiver.OnBeforeSerialize()
        {
            
        }

        // TODO: Remove the nasty hack here
        void IMessagePackSerializationCallbackReceiver.OnAfterDeserialize()
        {
            if(progress == null) return;
            switch(type)
            {
                case LegacyProgressReportType.Started:
                case LegacyProgressReportType.Progress:
                case LegacyProgressReportType.Reported:
                    throw new InvalidDataException();
                case LegacyProgressReportType.Compiled:
                    progress = MessagePackSerializer.Deserialize<LegacyCompilationResult>(MessagePackSerializer.Serialize(progress));
                    break;
                case LegacyProgressReportType.Finished:
                    progress = MessagePackSerializer.Deserialize<LegacyOverallResult>(MessagePackSerializer.Serialize(progress));
                    break;
            }
        }
    }
}