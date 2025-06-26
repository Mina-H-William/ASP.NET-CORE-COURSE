using Microsoft.AspNetCore.Mvc;
using ViewComponentExample.Models;

namespace ViewComponentExample.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/about")]
        public IActionResult About()
        {
            return View();
        }

        [Route("/friendslist")]
        public IActionResult LoadFriendList()
        {
            PersonGridModel personGridModel = new PersonGridModel()
            {
                GridTitle = "Persons",
                Persons = new List<Person> {
                    new Person { PersonName = "Mina", JobTitle = "Manger" },
                    new Person { PersonName = "Hany", JobTitle = "Asst.Manger" },
                    new Person { PersonName = "eslam", JobTitle = "Developer" }
                }
            };
            return ViewComponent("Grid", new { grid = personGridModel });
        }
    }
}
