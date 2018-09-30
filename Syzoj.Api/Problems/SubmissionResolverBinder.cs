using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Syzoj.Api.Problems
{
    public class SubmissionResolverBinder : IModelBinder
    {
        private readonly IProblemResolverProvider provider;

        public SubmissionResolverBinder(IProblemResolverProvider provider)
        {
            this.provider = provider;
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
            bindingContext.ModelState.SetModelValue("problemsetId", problemsetIdValue);
            Guid problemsetId;
            if(!Guid.TryParse(problemsetIdValue.FirstValue, out problemsetId))
            {
                bindingContext.ModelState.TryAddModelError(
                    "problemsetId",
                    "problemsetId is invalid."
                );
                return;
            }

            var submissionIdValue = bindingContext.ValueProvider.GetValue("submissionId");
            if(submissionIdValue == ValueProviderResult.None)
                return;
            bindingContext.ModelState.SetModelValue("submissionId", submissionIdValue);
            var id = submissionIdValue.FirstValue;
            Guid submissionId;
            if(!Guid.TryParse(submissionIdValue.FirstValue, out submissionId))
            {
                bindingContext.ModelState.TryAddModelError(
                    "submissionId",
                    "submissionId is invalid."
                );
                return;
            }

            var submissionResolver = await provider.GetSubmissionResolver(problemsetId, submissionId);
            if(submissionResolver == null)
            {
                bindingContext.ModelState.TryAddModelError(
                    "submissionId",
                    "Submission with specified submissionId does not exist in the problemset."
                );
                return;
            }

            bindingContext.Result = ModelBindingResult.Success(submissionResolver);
        }
    }
}