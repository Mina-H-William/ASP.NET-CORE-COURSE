using Microsoft.AspNetCore.Mvc.ModelBinding;
using modelValidationExample.Models;

namespace modelValidationExample.CustomModelBinders
{
    public class PersonModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Person person = new Person();

            if (bindingContext.ValueProvider.GetValue("FirstName").Length > 0)
            {
                person.PersonName = bindingContext.ValueProvider.GetValue("FirstName").FirstValue;

                if (bindingContext.ValueProvider.GetValue("LastName").Length > 0)
                {
                    person.PersonName += bindingContext.ValueProvider.GetValue("LastName").FirstValue;
                }
            }

            // others properties will be null as i didn't pass it to person object 
            // so when add custom modelbinder we should add all the other properties here in custom model binder
            // so add them like if to check bindingcontext have this value and if exist add it in person object like this 

            if (bindingContext.ValueProvider.GetValue("Email").Length > 0)
            {
                person.Email = bindingContext.ValueProvider.GetValue("Email").FirstValue;
            }

            // continue like this for all properties of object (person) that you want to catch it

            bindingContext.Result = ModelBindingResult.Success(person);

            return Task.CompletedTask;
        }
    }
}
