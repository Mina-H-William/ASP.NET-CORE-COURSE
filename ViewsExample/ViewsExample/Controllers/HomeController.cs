using Microsoft.AspNetCore.Mvc;
using ViewsExample.Models;

namespace ViewsExample.Controllers
{
    public class HomeController : Controller
    {
        //[Route("home")]
        //[Route("/")]
        //public IActionResult Index()
        //{
        //    ViewData["appTitle"] = "Asp.Net Core Demo App";
        //    List<Person> people = new List<Person>()
        //    {
        //        new Person() { Name = "Mina", DateOfBirth = Convert.ToDateTime("2002-09-30"),gender=Gender.Male },
        //        new Person() { Name = "Hany", DateOfBirth = Convert.ToDateTime("2010-09-30"),gender=Gender.Other },
        //        new Person() { Name = "William", DateOfBirth = Convert.ToDateTime("2015-09-30"),gender=Gender.Female }
        //    };
        //    ViewData["people"] = people;
        //    return View();
        //}

        [Route("/")]
        public IActionResult Test()
        {
            List<Person> people = new List<Person>()
                {
                    new Person() { Name = "Mina", DateOfBirth = Convert.ToDateTime("2002-09-30"),gender=Gender.Male },
                    new Person() { Name = "Hany", DateOfBirth = Convert.ToDateTime("2010-09-30"),gender=Gender.Male },
                    new Person() { Name = "William", DateOfBirth = Convert.ToDateTime("2015-09-30"),gender=Gender.Male },
                    new Person() { Name = "soha", DateOfBirth = Convert.ToDateTime("2020-09-30"),gender=Gender.Female }
                };
            // Both ViewData and ViewBag store values in same place 
            // so values can be accessed from both
            //ViewData["people"] = people; // equal to the next line 
            //ViewBag.People = people;

            return View(people);
        }

        [Route("/home/shared")]
        public IActionResult Shared()
        {
            return View();
            // first it will chech /Views/Home/Shared
            // if it doesn't exist so it will go to check /Views/Shared/Shared
        }
    }
}
