using System;

namespace Syzoj.Api.Problems.Standard
{
    public class StandardJudgerSettings
    {
        public string SpecialJudgeLanguage { get; set; }
        public Guid SpecialJudge { get; set; }
        public string InteractorLanguage { get; set; }
        public Guid Interactor { get; set; }
        public string InputFileName { get; set; }
        public string OutputFileName { get; set; }
        
        /// <summary>
        /// Time limit of the test case.
        /// </summary>
        public TimeSpan TimeLimit { get; set; }

        /// <summary>
        /// Memory limit of the test case, in bytes.
        /// </summary>
        public long MemoryLimit { get; set; }
    }
}