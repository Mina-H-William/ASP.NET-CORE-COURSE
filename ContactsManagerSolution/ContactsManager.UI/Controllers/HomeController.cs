using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CRUDExample.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {

        }

        [AllowAnonymous] // Allow anonymous access to all actions in this controller
        [Route("Error")]
        public IActionResult Error()
        {
            //whenever an exception occurs (Hnadled or Not), the error will be passed to features <IExceptionHandlerPathFeature> 
            IExceptionHandlerPathFeature? exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature != null && exceptionHandlerPathFeature.Error != null)
            {
                ViewBag.ErrorMessage = exceptionHandlerPathFeature.Error.Message;
            }

            return View(); //Views/Home/Error.cshtml or Views/Shared/Error.cshtml
        }
    }
}
