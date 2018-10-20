using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syzoj.Api.Object;

namespace Syzoj.Api.Mvc
{
    public class ObjectBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var logger = bindingContext.HttpContext.RequestServices.GetRequiredService<ILogger<ObjectBinder>>();
            logger.LogDebug("Working");
            if(bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if(valueProviderResult == ValueProviderResult.None)
            {
                return;
            }
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
            
            var value = valueProviderResult.FirstValue;
            logger.LogDebug($"Get object {value}");
            Guid id;
            if(Guid.TryParse(value, out id))
            {
                var objectService = bindingContext.HttpContext.RequestServices.GetRequiredService<IObjectService>();
                IObject obj = await objectService.GetObject(id);
                if(obj != null && bindingContext.ModelType.IsAssignableFrom(obj.GetType()))
                {
                    logger.LogDebug($"Found object {obj}");
                    bindingContext.Result = ModelBindingResult.Success(obj);
                }
                else
                {
                    bindingContext.ModelState.TryAddModelError(
                        bindingContext.ModelName,
                        $"Object {id} does not exist or is not of expected type"
                    );
                }
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    $"Invalid GUID format"
                );
            }
        }
    }
}