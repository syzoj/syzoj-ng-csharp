using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Models.Responses;

namespace Syzoj.Api.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public ValidateModelAttribute()
        {
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new InvalidModelStateResponse() {
                    Errors = context.ModelState.SelectMany(ms => ms.Value.Errors.Select(me => new ModelStateError() {
                        Name = ms.Key,
                        ErrorMessage = me.ErrorMessage,
                    })),
                });
            }
            else
            {
                await next();
            }
        }
    }

    public class ValidationFailResult : ObjectResult
    {
        public ValidationFailResult(IActionResult res) : base(res)
        {
            StatusCode = 400;
        }
    }
}