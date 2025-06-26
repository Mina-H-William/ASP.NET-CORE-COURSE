using Microsoft.AspNetCore.Mvc;
using PartialViewsExample.Models;

namespace PartialViewsExample.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            //ViewData["ListTitle"] = "Citites";

            //ViewData["ListItems"] = new List<string>()
            //{
            //    "Paris",
            //    "Egypt",
            //    "Rome",
            //    "London"
            //};

            return View();
        }

        [Route("/about")]
        public IActionResult About()
        {
            return View();
        }

        [Route("/programming-languages")]
        public IActionResult ProgrammingLanguage()
        {
            ListModel model = new ListModel()
            {
                ListTitle = "Programming Languages List",
                ListItems = new List<string>
                {
                    "C++",
                    "c#",
                    "Python",
                    "Java"
                }
            };

            //*************************
            // a huge benefit of partail view when call it in action it return content dynamic without a refresh of page
            // calling it by javascript as in index.cshtml
            //*************************
            return PartialView("_ListPartialView", model);
        }
    }
}
