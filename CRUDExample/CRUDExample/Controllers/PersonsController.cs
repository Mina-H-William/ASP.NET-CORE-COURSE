using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers
{
    public class PersonsController : Controller
    {
        private readonly ICountriesService _countriesService;
        private readonly IPersonsService _personsService;

        public PersonsController(ICountriesService countriesService, IPersonsService personsService)
        {
            _countriesService = countriesService;
            _personsService = personsService;
        }

        [Route("/persons/index")]
        [Route("/")]
        public IActionResult Index(string searchBy, string? searchString,
            string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            // Filter
            ViewBag.SearchFields = new Dictionary<string, string>(){
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.Country), "Country" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.Address), "Address" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" }
            };
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            List<PersonResponse> filteredPersons = _personsService.GetFilteredPersons(searchBy, searchString);

            //Sort
            List<PersonResponse> finalPersons = _personsService.GetSortedPersons(filteredPersons, sortBy, sortOrder);

            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            return View(finalPersons);
        }

        [Route("persons/create")]
        [HttpGet]
        public IActionResult CreatePerson()
        {
            ViewBag.Countries = _countriesService.GetAllCountries();
            return View();
        }

        [Route("persons/create")]
        [HttpPost]
        public IActionResult CreatePerson(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Countries = _countriesService.GetAllCountries();
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }
            _personsService.AddPerson(personAddRequest);

            return RedirectToAction("Index", "Persons");
            //return LocalRedirect("/persons/index");
        }
    }
}
