using Microsoft.AspNetCore.Mvc;

namespace ControllerExample.Controllers
{
    public class RedirectController : Controller
    {
        //***********************************************************************************************************
        // when pass values in redirect to action (in object third parameter) it takes the values that represent in route
        // as route parameters and the othere values that not exist in route it consider them as query string
        // ************************************************************************************************************
        [Route("/new-url/{id}")]
        public IActionResult Index(int id, string name)
        {
            return Content($"redirect is completely done {name} has id: {id}");
        }
    }
}
