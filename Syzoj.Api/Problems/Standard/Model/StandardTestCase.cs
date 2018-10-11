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
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public Guid JudgerSettings { get; set; }
        /// <summary>
        /// A list of additional files to be placed in the working directory
        /// of the program.
        /// </summary>
        public IDictionary<string, Guid> Files { get; set; }
    }
}