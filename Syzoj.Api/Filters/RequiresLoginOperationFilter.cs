using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Syzoj.Api.Filters
{
    public class RequiresLoginOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<RequiresLoginAttribute>();
            
            if(authAttributes.Any())
                operation.Responses.Add("401", new Response { Description = "Login required."});
        }
    }
}