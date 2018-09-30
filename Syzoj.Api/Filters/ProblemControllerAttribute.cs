using System;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Syzoj.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProblemControllerAttribute : Attribute, IRouteTemplateProvider
    {
        public string Template => "api/problem/{problemsetId}/{problemId}";

        public int? Order => 0;

        public string Name => null;
    }
}