using System;
using System.Threading.Tasks;
using Syzoj.Api.Problems.Standard.Model;

namespace Syzoj.Api.Problems
{
    /// <summary>
    /// The interface defines a resolver that resolves some problem of the type.
    /// </summary>
    public interface IProblemResolver
    {
        /// <summary>
        /// The ID of the problem.
        /// </summary>
        Guid Id { get; }
    }
}