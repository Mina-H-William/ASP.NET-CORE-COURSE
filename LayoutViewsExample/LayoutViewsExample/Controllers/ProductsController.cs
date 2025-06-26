using Microsoft.AspNetCore.Mvc;

namespace LayoutViewsExample.Controllers
{
    public class ProductsController : Controller
    {
        [Route("/products")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/search-Products/{id?}")]
        public IActionResult Search(int? id)
        {
            ViewBag.id = id;
            return View();
        }

        [Route("/order-product")]
        public IActionResult Order()
        {
            return View();
        }
    }
}
