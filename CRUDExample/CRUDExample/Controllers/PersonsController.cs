using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers
{
    [Route("/[controller]")]
    public class PersonsController : Controller
    {
        private readonly ICountriesService _countriesService;
        private readonly IPersonsService _personsService;

        public PersonsController(ICountriesService countriesService, IPersonsService personsService)
        {
            _countriesService = countriesService;
            _personsService = personsService;
        }

        [Route("[action]")]
        [Route("/")]
        public async Task<IActionResult> Index(string searchBy, string? searchString,
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

            List<PersonResponse> filteredPersons = await _personsService.GetFilteredPersons(searchBy, searchString);

            //Sort
            List<PersonResponse> finalPersons = _personsService.GetSortedPersons(filteredPersons, sortBy, sortOrder);

            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            return View(finalPersons);
        }

        [Route("[action]")]
        [HttpGet]
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
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            // difference between Selectmany and select is that SelectMany flattens the collection (means reduced
            // nested list by one) in other meaning selectmany select from each item many items and put them in one list
            // and select just select one item (in arrays it select the whole array) from each item in the collection

            if (!ModelState.IsValid)
            {
                List<CountryResponse> Countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = Countries.Select(c =>
                    new SelectListItem() { Text = c.CountryName, Value = c.CountryID.ToString() });

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return View(personAddRequest);
            }
            await _personsService.AddPerson(personAddRequest);

            return RedirectToAction("Index", "Persons");
            //return LocalRedirect("/persons/index");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? person = await _personsService.GetPersonByPersonID(personID);
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
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdate)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> Countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = Countries.Select(c =>
                                    new SelectListItem() { Text = c.CountryName, Value = c.CountryID.ToString() });

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return View(personUpdate);
            }

            if (await _personsService.GetPersonByPersonID(personUpdate.PersonID) is null)
            {
                return RedirectToAction("index");
            }

            await _personsService.UpdatePerson(personUpdate);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);
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

            if ((await _personsService.GetPersonByPersonID(personUpdate.PersonID)) is null)
            {
                return RedirectToAction("Index");
            }

            if (!(await _personsService.DeletePerson(personUpdate.PersonID)))
                return View(personUpdate);

            return RedirectToAction("Index");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsPDF()
        {
            List<PersonResponse> persons = await _personsService.GetAllPersons();

            return new ViewAsPdf("PersonsPDF", persons, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 10, 10, 10), // Set margins for the PDF
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape // Set orientation to Landscape
            };
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream csvStream = await _personsService.GetPersonsCSVAuto();

            return File(csvStream, "application/octet-stream", "persons.csv");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream excelStream = await _personsService.GetPersonsExcel();

            // file type of xlsx is application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
            return File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }


    }
}
