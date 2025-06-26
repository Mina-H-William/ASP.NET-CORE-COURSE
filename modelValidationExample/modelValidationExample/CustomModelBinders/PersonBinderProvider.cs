using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using modelValidationExample.Models;

namespace modelValidationExample.CustomModelBinders
{
    public class PersonBinderProvider : IModelBinderProvider
    {
        // this used to make this [ModelBinder(BinderType = typeof(PersonModelBinder))] called auto for all Person models
        // in model binding (no need to call it) and no need for [FromBody] also as it bind as the function say
        // but this providere should be added in program.cs in addcontrollers in services in options
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(Person))
            {
                return new BinderTypeModelBinder(typeof(Person));
            }
            return null;
        }
    }
}
