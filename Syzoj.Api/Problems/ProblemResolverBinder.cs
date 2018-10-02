using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Syzoj.Api.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Syzoj.Api.Problems
{
    public class ProblemResolverBinder : IModelBinder
    {
        private readonly IProblemResolverProvider provider;
        private readonly ApplicationDbContext context;

        public ProblemResolverBinder(IProblemResolverProvider provider, ApplicationDbContext context)
        {
            this.provider = provider;
            this.context = context;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if(bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var problemsetIdValue = bindingContext.ValueProvider.GetValue("problemsetId");
            if(problemsetIdValue == ValueProviderResult.None)
                return;
            
            var id = problemsetIdValue.FirstValue;
            Guid problemsetId;
            if(!Guid.TryParse(id, out problemsetId))
            {
                bindingContext.ModelState.TryAddModelError(
                    "problemsetId",
                    "problemsetId is invalid."
                );
                return;
            }
            // TODO: Check whether problemsetId exists

            var problemNameValue = bindingContext.ValueProvider.GetValue("problemName");
            if(problemNameValue == ValueProviderResult.None)
                return;
            
            var problemName = problemNameValue.FirstValue;
            
            // TODO: Use Redis for this
            var problemId = await context.ProblemsetProblems
                .Where(psp => psp.ProblemsetId == problemsetId && psp.ProblemsetProblemId == problemName)
                .Select(psp => psp.ProblemId)
                .FirstOrDefaultAsync();
            if(problemId == null)
            {
                bindingContext.ModelState.TryAddModelError(
                    "problemName",
                    "Problem with specified name does not exist."
                );
                return;
            }

            var problemResolver = await provider.GetProblemResolver(problemId);
            bindingContext.Result = ModelBindingResult.Success(problemResolver);
        }
    }
}