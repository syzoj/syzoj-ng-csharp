using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Syzoj.Api.Problems
{
    public class ProblemResolverBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if(context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if(context.Metadata.ModelType == typeof(IProblemResolver))
            {
                return new BinderTypeModelBinder(typeof(ProblemResolverBinder));
            }

            if(context.Metadata.ModelType == typeof(ISubmissionResolver))
            {
                return new BinderTypeModelBinder(typeof(SubmissionResolverBinder));
            }

            return null;
        }
    }
}