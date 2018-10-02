using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Syzoj.Api.Problems
{
    public interface IProblemsetManager
    {
        /// <summary>
        /// The ID of the problemset.
        /// </summary>
        Guid Id { get; }
    }
}