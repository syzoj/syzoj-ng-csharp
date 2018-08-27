namespace Syzoj.Api.Models.Runner
{
    public enum LegacyTestcaseResultType
    {
        Accepted = 1,
        WrongAnswer = 2,
        PartiallyCorrect = 3,
        MemoryLimitExceeded = 4,
        TimeLimitExceeded = 5,
        OutputLimitExceeded = 6,
        FileError = 7, // The output file does not exist
        RuntimeError = 8,
        JudgementFailed = 9, // Special Judge or Interactor fails
        InvalidInteraction = 10
        
    }
}