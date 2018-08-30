using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MessagePack;
using Syzoj.Api.Models.Runner;

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
        [Required]
        public DateTime? SubmissionTime { get; set; }
        [Column("SubmissionSummary")]
        public byte[] _SubmissionSummary { get; set; }
        [Column("SubmissionData")]
        public byte[] _SubmissionData { get; set; }

        public T GetSubmissionSummary<T>()
        {
            return MessagePackSerializer.Deserialize<T>(_SubmissionSummary);
        }
        public void SetSubmissionSummary<T>(T SubmissionSummary)
        {
            _SubmissionSummary = MessagePackSerializer.Serialize<T>(SubmissionSummary);
        }
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

    public class SubmissionData<T1, T2>
    {
        public T1 Summary { get; set; }
        public T2 Data { get; set; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacyCodeSummary
    {
        public int CodeSize { get; set; }
        public string Language { get; set; }
        public decimal Score { get; set; }
        public int TotalTime { get; set; }
        public int MaxMemory { get; set; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacyCodeData
    {
        public string Code { get; set; }
        public LegacyOverallResult Result { get; set; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacyAnswerSubmissionSummary
    {
        public decimal Score { get; set; }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class SyzojLegacyAnswerSubmissionData
    {
        public byte[] AnswerHash { get; set; }
        public LegacyOverallResult Result { get; set; }
    }
}