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
            Guid id;
            if(Guid.TryParse(value, out id))
            {
                var objectService = bindingContext.HttpContext.RequestServices.GetRequiredService<IObjectService>();
                IObject obj = await objectService.GetObject(id);
                if(obj != null && bindingContext.ModelType.IsAssignableFrom(obj.GetType()))
                {
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