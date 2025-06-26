using Microsoft.AspNetCore.Mvc;

namespace ViewsExample.Controllers
{
    public class ProductsController : Controller
    {
        [Route("/products/all")]
        public IActionResult All()
        {
            return View();
        }

        [Route("/products/shared")]
        public IActionResult Shared()
        {
            return View();
            // first it will chech /Views/Products/Shared
            // if it doesn't exist so it will go to check /Views/Shared/Shared
        }
    }
}
