using Microsoft.AspNetCore.Mvc;
using modelValidationExample.CustomModelBinders;
using modelValidationExample.Models;

namespace modelValidationExample.Controllers
{
    //[ApiController]
    public class HomeController : Controller
    {
        //[FromBody] is required in arguments of actions in Default Controllers if the data are supplied
        // in Request body as JSON or XML
        // but if we write on controller [ApiController] (for Apis Controllers) so not require to write [FromBody] it work automatic

        // for json and xml if it has a nested structure like objects we need wrapper to data

        //[ModelBinder(BinderType = typeof(PersonModelBinder))] , [FromBody] both removed as we add model binder provider
        [Route("/register")]
        public IActionResult Register(Person person)
        {
            if (!ModelState.IsValid)
            {
                string errors = string.Join("\n", ModelState.Values
                                                           .SelectMany(value => value.Errors)
                                                           .Select(error => error.ErrorMessage)
                                                           .ToList());
                return BadRequest(errors);
            }
            return Content($"{person}");
        }
    }
}
