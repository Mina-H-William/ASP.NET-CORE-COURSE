using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;

        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //To do: Add After action logic here
            _logger.LogInformation("{FilterName}: {MethodName} called", nameof(PersonsListActionFilter), nameof(OnActionExecuted));

            PersonsController personsController = (PersonsController)context.Controller;

            IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];

            if (parameters != null)
            {
                if (parameters.ContainsKey("searchBy"))
                    personsController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["searchBy"]);

                if (parameters.ContainsKey("searchString"))
                    personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);

                if (parameters.ContainsKey("sortBy"))
                    personsController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["sortBy"]);
                else personsController.ViewData["CurrentSortBy"] = nameof(PersonResponse.PersonName);

                if (parameters.ContainsKey("sortOrder"))
                    personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrder"]);
                else personsController.ViewData["CurrentSortOrder"] = nameof(SortOrderOptions.ASC);
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //To do: Add Before action logic here
            _logger.LogInformation("{FilterName}: {MethodName} called", nameof(PersonsListActionFilter), nameof(OnActionExecuting));

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>() {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.Country),
                        nameof(PersonResponse.Address)
                    };

                    // reset searchBy to PersonName if it is not in the list of options
                    if (!searchByOptions.Any(tmp => tmp == searchBy))
                    {
                        _logger.LogInformation("searchBy actual value {searchBy}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        _logger.LogInformation("searchBy updated value {searchBy}", nameof(PersonResponse.PersonName));
                    }
                }
            }

            // Store the action arguments in HttpContext.Items for later use in the view or other filters
            context.HttpContext.Items["arguments"] = context.ActionArguments;
        }
    }
}
