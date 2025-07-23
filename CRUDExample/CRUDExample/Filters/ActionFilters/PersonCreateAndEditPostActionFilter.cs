using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CRUDExample.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesService _countriesService;

        public PersonCreateAndEditPostActionFilter(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is PersonsController personsController)
            {
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> Countries = await _countriesService.GetAllCountries();
                    personsController.ViewBag.Countries = Countries.Select(c =>
                        new SelectListItem() { Text = c.CountryName, Value = c.CountryID.ToString() });

                    personsController.ViewBag.Errors = personsController.ModelState.Values
                                                                                   .SelectMany(v => v.Errors)
                                                                                   .Select(e => e.ErrorMessage).ToList();

                    var personRequest = context.ActionArguments["personRequest"];

                    context.Result = personsController.View(personRequest); // short-circuits

                    return;
                }
            }

            await next(); // continue to the next action in the pipeline

        }
    }
}
