using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace DIExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICitiesService _CitiesService;
        private readonly ICitiesService _CitiesService2;


        private readonly IServiceScopeFactory _serviceScopeFactory;


        public HomeController(ICitiesService citiesService, ICitiesService citiesService2, IServiceScopeFactory serviceScopeFactory)
        {
            _CitiesService = citiesService;
            _serviceScopeFactory = serviceScopeFactory;
            _CitiesService2 = citiesService2;
        }

        //[FromServices] ICitiesService _CitiesService in Arguments of index if you want to inject to specific method only
        [Route("/")]
        public IActionResult Index()
        {
            List<string> Cities = _CitiesService.GetCities();

            ViewBag.CititesService_Id1 = _CitiesService.ServiceInstanceId;
            ViewBag.CititesService_Id2 = _CitiesService2.ServiceInstanceId;

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                //inject services as it will create new object of service in scope lifetime as it new scope

                ICitiesService? citiesService = scope.ServiceProvider.GetService<ICitiesService>();

                //DB work

                ViewBag.CititesService_Id3 = citiesService?.ServiceInstanceId;

            }// end of scope: its call services.dispose() auto (Example: close connection of database)

            return View(Cities);
        }
    }
}
