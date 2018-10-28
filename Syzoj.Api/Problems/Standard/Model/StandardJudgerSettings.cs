using System;
using System.ComponentModel.DataAnnotations;
using MessagePack;
using Syzoj.Api.Mvc;

namespace Syzoj.Api.Problems.Standard
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class StandardJudgerSettings
    {
        public string SpecialJudgeLanguage { get; set; }
        public string SpecialJudge { get; set; }
        public string InteractorLanguage { get; set; }
        public string Interactor { get; set; }
        public string InputFileName { get; set; }
        public string OutputFileName { get; set; }
        
        /// <summary>
        /// Time limit of the test case, in milliseconds.
        /// </summary>
        [Positive]
        public long TimeLimit { get; set; }

        /// <summary>
        /// Memory limit of the test case, in bytes.
        /// </summary>
        [Positive]
        public long MemoryLimit { get; set; }
    }
}