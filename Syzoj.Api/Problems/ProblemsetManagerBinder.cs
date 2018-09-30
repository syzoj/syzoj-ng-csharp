using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Syzoj.Api.Problems
{
    public class ProblemsetManagerBinder : IModelBinder
    {
        private readonly IProblemsetManagerProvider provider;

        public ProblemsetManagerBinder(IProblemsetManagerProvider provider)
        {
            this.provider = provider;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if(bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var value = bindingContext.ValueProvider.GetValue("problemsetId");
            if(value == ValueProviderResult.None)
                return;
            
            bindingContext.ModelState.SetModelValue("problemsetId", value);
            var id = value.FirstValue;
            Guid problemsetId;
            if(!Guid.TryParse(id, out problemsetId))
            {
                bindingContext.ModelState.TryAddModelError(
                    "problemsetId",
                    "problemsetId is invalid."
                );
                return;
            }

            var problemsetManager = await provider.GetProblemsetManager(problemsetId);
            if(problemsetManager == null)
            {
                bindingContext.ModelState.TryAddModelError(
                    "problemsetId",
                    "Problemset with specified problemsetId does not exist."
                );
                return;
            }

            bindingContext.Result = ModelBindingResult.Success(problemsetManager);
        }
    }
}