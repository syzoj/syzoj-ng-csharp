using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;

namespace Syzoj.Api.Models.Data
{
    public class ProblemSubmission
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual ProblemSet ProblemSet { get; set; }
        public int ProblemSetId { get; set; }
        public virtual Problem Problem { get; set; }
        public int ProblemId { get; set; }
        public virtual ProblemSetProblem ProblemSetProblem { get; set; }
        public ProblemSubmissionStatus Status { get; set; }
        public ProblemSubmissionType Type { get; set; }
        [Column("SubmissionData")]
        public byte[] _SubmissionData { get; set; }

        public T GetSubmissionData<T>()
        {
            return MessagePackSerializer.Deserialize<T>(_SubmissionData);
        }
        public void SetSubmissionData<T>(T SubmissionData)
        {
            _SubmissionData = MessagePackSerializer.Serialize<T>(SubmissionData);
        }
    }

    public enum ProblemSubmissionStatus
    {
        // Queue status
        NotJudged = 1,
        // Manually aborted
        Aborted = 2,
        Waiting = 3,
        Running = 4,

        // Judgement done
        UnknownStatus = 10001,
        Accepted = 10002,
        WrongAnswer = 10003,
        PartiallyCorrect = 10004,
        TimeLimitExceeded = 10005,
        MemoryLimitExceeded = 10006,
        RuntimeError = 10007,
        OutputLimitExceeded = 10008,
        DangerousSyscall = 10009,
        // Failure status
        JudgementDenied = 20001,
        SpecialJudgeRuntimeError = 20002,
        
    }

    public enum ProblemSubmissionType
    {
        SyzojLegacyCode = 1,
        SyzojLegacySubmitAnswer = 2,
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacyCodeSubmissionData
    {

    }
}