using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Syzoj.Api.Problems
{
    public class ProblemResolverBinder : IModelBinder
    {
        private readonly IProblemResolverProvider provider;

        public ProblemResolverBinder(IProblemResolverProvider provider)
        {
            this.provider = provider;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if(bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var value = bindingContext.ValueProvider.GetValue("problemId");
            if(value == ValueProviderResult.None)
                return;
            
            bindingContext.ModelState.SetModelValue("problemId", value);
            var id = value.FirstValue;
            Guid problemId;
            if(!Guid.TryParse(id, out problemId))
            {
                bindingContext.ModelState.TryAddModelError(
                    "problemId",
                    "problemId is invalid."
                );
                return;
            }

            var problemResolver = await provider.GetProblemResolver(problemId);
            if(problemResolver == null)
            {
                bindingContext.ModelState.TryAddModelError(
                    "problemsetId",
                    "Problemset with specified problemsetId does not exist."
                );
                return;
            }

            bindingContext.Result = ModelBindingResult.Success(problemResolver);
        }
    }
}