using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Syzoj.Api.Filters
{
    public class SessionHeaderOperationFilter : IOperationFilter
    {
        private class Parameter : IParameter
        {
            public string Name { get; set;}
            public string In { get; set; }
            public string Description { get; set; }
            public bool Required { get; set; }

            public Dictionary<string, object> Extensions { get; }
        }
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<RequiresLoginAttribute>();
            
            operation.Parameters.Add(new Parameter() {
                Name = "Session",
                @In = "header",
                Description = "Session token",
                Required = authAttributes.Any(),
            });
        }
    }
}