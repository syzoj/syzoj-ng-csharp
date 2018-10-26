using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Syzoj.Api.Data;

namespace Syzoj.Api.Mvc
{
    public class SaveChangesActionFitler : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next();
            await context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>().SaveChangesAsync();
        }
    }
}