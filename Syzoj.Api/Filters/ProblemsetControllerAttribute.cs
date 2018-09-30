using System;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Syzoj.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProblemsetControllerAttribute : Attribute, IRouteTemplateProvider
    {
        public string Template => "api/problemset/{problemsetId}";

        public int? Order => 0;

        public string Name => null;
    }
}