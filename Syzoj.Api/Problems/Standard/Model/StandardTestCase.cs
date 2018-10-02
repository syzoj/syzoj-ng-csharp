using System;
using System.Collections.Generic;
using MessagePack;

namespace Syzoj.Api.Problems.Standard.Model
{
    /// <summary>
    /// Decribes a single testcase.
    /// </summary>
    [MessagePackObject(keyAsPropertyName: true)]
    public class StandardTestCase
    {
        /// <summary>
        /// The name of input file, null if input should come from stdin.
        /// </summary>
        public string InputFileName { get; set; }

        /// <summary>
        /// The name of output file, null if output should go to stdout.
        /// </summary>
        public string OutputFileName { get; set; }

        /// <summary>
        /// The contents of input file.
        /// </summary>
        public Guid InputFile { get; set; }

        /// <summary>
        /// Time limit of the test case.
        /// </summary>
        public TimeSpan TimeLimit { get; set; }

        /// <summary>
        /// Memory limit of the test case, in bytes.
        /// </summary>
        public long MemoryLimit { get; set; }

        /// <summary>
        /// A list of additional files to be placed in the working directory
        /// of the program.
        /// </summary>
        public IDictionary<string, Guid> Files { get; set; }
    }
}