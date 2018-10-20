using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syzoj.Api.Object;

namespace Syzoj.Api.Mvc
{
    public class ObjectBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if(context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if(typeof(IObject).IsAssignableFrom(context.Metadata.ModelType))
            {
                return new ObjectBinder();
            }

            return null;
        }
    }
}