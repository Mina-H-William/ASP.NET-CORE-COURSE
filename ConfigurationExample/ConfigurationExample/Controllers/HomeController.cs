using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ConfigurationExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly WeatherApiOptions _options;

        public HomeController(IConfiguration configuration, IOptions<WeatherApiOptions> weatherapioptions)
        {
            _configuration = configuration;
            _options = weatherapioptions.Value;
        }


        [Route("/")]
        public IActionResult Index()
        {
            //ViewBag.mykey = _configuration["mykey"];
            //ViewBag.apikey = _configuration.GetValue<string>("apikey", "notfound");

            //ViewBag.clientid = _configuration["weatherapi:clientid"];
            //ViewBag.clientsecret = _configuration.GetValue<string>("weatherapi:clientsecret", "client secret notfound");

            //IConfigurationSection weatherapi = _configuration.GetSection("weatherapi");

            //ViewBag.clientid = weatherapi["clientid"];
            //ViewBag.clientsecret = weatherapi.GetValue<string>("clientsecret", "client secret notfound");

            // Get -> create new object and add value to it.
            //WeatherApiOptions options = _configuration.GetSection("weatherapi").Get<WeatherApiOptions>();

            // Bind -> it add values to exist object.
            //WeatherApiOptions options = new WeatherApiOptions();
            //_configuration.GetSection("weatherapi").Bind(options);

            //ViewBag.clientid = options.ClientID;
            //ViewBag.clientsecret = options.ClientSecret;

            ViewBag.clientid = _options.ClientID;
            ViewBag.clientsecret = _options.ClientSecret;


            return View();
        }
    }
}
