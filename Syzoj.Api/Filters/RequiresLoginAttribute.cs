using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Syzoj.Api.Models.Responses;
using Syzoj.Api.Services;

namespace Syzoj.Api.Filters
{
    public class RequiresLoginAttribute : TypeFilterAttribute
    {
        public RequiresLoginAttribute() : base(typeof(RequiresLoginAttributeImpl))
        {

        }

        private class RequiresLoginAttributeImpl : IAsyncActionFilter
        {
            private readonly ISessionManager sessionManager;
            public RequiresLoginAttributeImpl(ISessionManager sessionManager)
            {
                this.sessionManager = sessionManager;
            }
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if(!sessionManager.IsAuthenticated())
                {
                    var result = new ObjectResult(new BaseResponse() {
                        Status = "Fail",
                        Code = 0,
                        Message = "Login required.",
                    });
                    result.StatusCode = 401;
                    context.Result = result;
                }
                else
                {
                    await next();
                }
            }
        }
    }
}