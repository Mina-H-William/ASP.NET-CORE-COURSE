using Microsoft.AspNetCore.Mvc;
using ControllerExample.Models;
using System.Security.Cryptography;

namespace ControllerExample.Controllers
{
   
    public class HomeController : Controller
    {
        [Route("/")]
        public ContentResult Index()
        {
            //return new ContentResult() { 
            //    Content = "Hello from Index",
            //    ContentType = "text/plain"
            //};
            return Content("Hello from Index", "text/plain");   // The Content is a method in Controllerbase (parent of Controller class)
        }

        [Route("/person")]
        public JsonResult Person()
        {
            var person = new Person() { Id = Guid.NewGuid(), FirstName = "Mina", LastName = "Hany", Age=25 };
            //return new JsonResult(person);
            return Json(person);
        }

        [Route("contact/{mobile:regex(^\\d{{10}}$)}")]
        public string Contact(string? mobile)
        {
            return $"Hello from Contact: {mobile}";
        }

        // use virtualfileresult if file in wwwroot (relative path, content Type)
        [Route("file-download")]
        public VirtualFileResult FileDownload()
        {
            //return new VirtualFileResult("/sample.pdf","application/pdf");
            return File("/sample.pdf", "application/pdf");
        }

        // use Physicalfileresult if file not in wwwroot (absolute path, content Type)
        [Route("file-download2")]
        public IActionResult FileDownload2()
        {
            //return new PhysicalFileResult(@"D:\My Resume\PDF compressed\Mina Hany.pdf", "application/pdf");
            return PhysicalFile(@"D:\My Resume\PDF compressed\Mina Hany.pdf", "application/pdf");
        }

        // use FileContentResult if file is array of Bytes (Bytes, content Type)
        [Route("file-download3")]
        public IActionResult FileDownload3()
        {
            Byte[] Bytes = System.IO.File.ReadAllBytes(@"D:\My Resume\PDF compressed\Mina Hany.pdf");
            //return new FileContentResult(Bytes, "application/pdf");
            return File(Bytes, "application/pdf");
        }

        //use redirecttoaction when want to redirect specific action and change url to route of new action
        [Route("old-url")]
        public IActionResult OldUrl()
        {
            //return new RedirectToActionResult("index", "redirect", new { id = 10, name = "mina" }); // not permanent (302)
            return RedirectToAction("index", "redirect", new {id=10,name="mina"}); // shortcut for above
            //return new RedirectToActionResult("index", "redirect", new { id = 10, name = "mina" }, true); // permanent(301)
            //return RedirectToActionPermanent("index", "redirect", new { id = 10, name = "mina" }); // shortcut for above

            // use local redirect to go to url not action and controller (write url) within the application

            //return new LocalRedirectResult("new-url");  // not permanent
            //return LocalRedirect("/new-url");
            //return new LocalRedirectResult("new-url",true); // permanent
            //return LocalRedirectPermanent("new-url");

            // usse redirect to go to new url can be within application or to another application

            //return new RedirectResult("new-url"); // not permanent
            //return Redirect("new-url");
            //return new RedirectResult("new-url",true); // permanent
            //return RedirectPermanent("new-url");



            //permanent mean if old url exist in google search or bookmarks in users browsers and want to change it to the new one
        }

    }
}
