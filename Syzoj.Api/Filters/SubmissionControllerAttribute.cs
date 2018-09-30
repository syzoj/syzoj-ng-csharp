using System;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Syzoj.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SubmissionControllerAttribute : Attribute, IRouteTemplateProvider
    {
        public string Template => "api/submission/{problemsetId}/{submissionId}";

        public int? Order => 0;

        public string Name => null;
    }
}