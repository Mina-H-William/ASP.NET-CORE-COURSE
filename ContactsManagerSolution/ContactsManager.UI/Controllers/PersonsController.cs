using CRUDExample.Filters;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.AuthorizationFilters;
using CRUDExample.Filters.ExceptionFilters;
using CRUDExample.Filters.ResourceFilters;
using CRUDExample.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers
{
    [Route("/[controller]")]
    //[TypeFilter(typeof(ResponseHeaderActionFilter),
    //            Arguments = new object[] { "X-Custom-Key-From-Controller", "Custom-Value-From-Controller", 3 })]
    //[TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonsAlwaysRunResultFilter))]
    public class PersonsController : Controller
    {
        private readonly ICountriesService _countriesService;
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(ICountriesService countriesService, ILogger<PersonsController> logger,
                                IPersonsAdderService personsAdderService, IPersonsDeleterService personsDeleterService,
                                IPersonsUpdaterService personsUpdaterService, IPersonsSorterService personsSorterService,
                                IPersonsGetterService personsGetterService)
        {
            _countriesService = countriesService;
            _personsGetterService = personsGetterService;
            _personsDeleterService = personsDeleterService;
            _personsUpdaterService = personsUpdaterService;
            _personsSorterService = personsSorterService;
            _personsAdderService = personsAdderService;
            _logger = logger;
        }

        [Route("[action]")]
        [Route("/")]
        [ServiceFilter(typeof(PersonsListActionFilter))] // must add filter class to IOC container as a service to work
        //[TypeFilter(typeof(ResponseHeaderActionFilter),
        //            Arguments = new object[] { "X-Custom-Key-From-Action", "Custom-Value-From-Action", 1 })]
        //[ResponseHeaderActionFilter("X-Custom-Key-From-Action", "Custom-Value-From-Action", 1)]
        [ResponseHeaderFilterFactory("X-Custom-Key-From-Action", "Custom-Value-From-Action", 1)]
        [TypeFilter(typeof(PersonsListResultFilter))]
        [SkipFilter]
        public async Task<IActionResult> Index(string searchBy, string? searchString,
            string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            _logger.LogInformation("Index action method of PersonsController");
            _logger.LogDebug($"searchBy: {searchBy}, searchstring: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");

            // Filter
            ViewBag.SearchFields = new Dictionary<string, string>(){
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.Country), "Country" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.Address), "Address" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" }
            };

            List<PersonResponse> filteredPersons = await _personsGetterService.GetFilteredPersons(searchBy, searchString);

            //Sort
            List<PersonResponse> finalPersons = _personsSorterService.GetSortedPersons(filteredPersons, sortBy, sortOrder);

            // put them in filter of action filter in onexcuted method
            //ViewBag.CurrentSearchBy = searchBy;
            //ViewBag.CurrentSearchString = searchString;

            //ViewBag.CurrentSortBy = sortBy;
            //ViewBag.CurrentSortOrder = sortOrder.ToString();

            return View(finalPersons);
        }

        [Route("[action]")]
        [HttpGet]
        //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "my-Key-From-Action", "my-Value-From-Action", 3 })]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> Countries = await _countriesService.GetAllCountries();

            //ViewBag.Countries = Countries.Select(c =>
            //new SelectListItem() { Text = c.CountryName, Value = c.CountryID.ToString() });

            // Best approach to create a SelectList for dropdowns in views (2nd argument for value and 3rd for text shown)
            ViewBag.Countries = new SelectList(Countries, nameof(CountryResponse.CountryID), nameof(CountryResponse.CountryName));

            //new SelectListItem()
            //{
            //    Text = "mina",
            //    Value ="1"
            //};
            //<Option value="1"> Mina</Option>

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(FeatureDisableResourceFilter), Arguments = new object[] { false })]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            // difference between Selectmany and select is that SelectMany flattens the collection (means reduced
            // nested list by one) in other meaning selectmany select from each item many items and put them in one list
            // and select just select one item (in arrays it select the whole array) from each item in the collection

            #region added to Filter
            //if (!ModelState.IsValid)
            //{
            //    List<CountryResponse> Countries = await _countriesService.GetAllCountries();
            //    ViewBag.Countries = Countries.Select(c =>
            //        new SelectListItem() { Text = c.CountryName, Value = c.CountryID.ToString() });

            //    ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

            //    return View(personRequest);
            //}
            #endregion

            await _personsAdderService.AddPerson(personRequest);

            return RedirectToAction("Index", "Persons");
            //return LocalRedirect("/persons/index");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? person = await _personsGetterService.GetPersonByPersonID(personID);
            if (person is null)
            {
                return RedirectToAction("Index");
            }
            PersonUpdateRequest personUpdateRequest = person.ToPersonUpdateRequest();
            List<CountryResponse> Countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = Countries.Select(c =>
                                new SelectListItem() { Text = c.CountryName, Value = c.CountryID.ToString() });

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{pesonID}")]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            #region added to Filter
            //if (!ModelState.IsValid)
            //{
            //    List<CountryResponse> Countries = await _countriesService.GetAllCountries();
            //    ViewBag.Countries = Countries.Select(c =>
            //                        new SelectListItem() { Text = c.CountryName, Value = c.CountryID.ToString() });

            //    ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

            //    return View(personRequest);
            //}
            #endregion

            if (await _personsGetterService.GetPersonByPersonID(personRequest.PersonID) is null)
            {
                return RedirectToAction("index");
            }

            await _personsUpdaterService.UpdatePerson(personRequest);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid personID)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personID);
            if (personResponse is null)
            {
                return RedirectToAction("Index");
            }

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdate)
        {

            if ((await _personsGetterService.GetPersonByPersonID(personUpdate.PersonID)) is null)
            {
                return RedirectToAction("Index");
            }

            if (!(await _personsDeleterService.DeletePerson(personUpdate.PersonID)))
                return View(personUpdate);

            return RedirectToAction("Index");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsPDF()
        {
            List<PersonResponse> persons = await _personsGetterService.GetAllPersons();

            return new ViewAsPdf("PersonsPDF", persons, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 10, 10, 10), // Set margins for the PDF
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape // Set orientation to Landscape
            };
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream csvStream = await _personsGetterService.GetPersonsCSVAuto();

            return File(csvStream, "application/octet-stream", "persons.csv");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream excelStream = await _personsGetterService.GetPersonsExcel();

            // file type of xlsx is application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
            return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
    }
}
