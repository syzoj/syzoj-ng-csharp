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
            var logger = context.Services.GetRequiredService<ILogger<ObjectBinderProvider>>();
            logger.LogDebug("GetBinder");
            if(context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            logger.LogDebug(context.Metadata.ModelType.ToString());
            if(typeof(IObject).IsAssignableFrom(context.Metadata.ModelType))
            {
                logger.LogDebug("Applies");
                return new ObjectBinder();
            }

            return null;
        }
    }
}