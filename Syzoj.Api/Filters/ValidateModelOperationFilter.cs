using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Syzoj.Api.Filters
{
    public class ValidateModelOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<ValidateModelAttribute>();
            
            if(authAttributes.Any())
            {
                operation.Responses.Add("400", new Response { Description = "Invalid request."});
            }
        }
    }
}