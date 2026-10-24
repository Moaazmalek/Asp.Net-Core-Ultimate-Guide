
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using ContactsManager.UI.Controllers;
using Microsoft.AspNetCore.Mvc.Filters; 

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
            //To do: add after logic here 
            //_logger.LogInformation("PersonsListActionFilter.OnActionExecuted method");
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter),nameof(OnActionExecuted));
            PersonsController personsController = (PersonsController)context.Controller;
            IDictionary<string, object?>? parameters = (IDictionary<string, object?>?)context.HttpContext.Items["arguments"];
            if (parameters != null)
            {
                if (parameters.TryGetValue("searchBy", out object? searchBy))
                {
                    personsController.ViewData["CurrentSearchBy"] = Convert.ToString(searchBy);
                }
                if (parameters.TryGetValue("searchString", out object? searchString))
                {
                    personsController.ViewData["CurrentSearchString"] = Convert.ToString(searchString);
                }
                if (parameters.TryGetValue("sortBy", out object? sortBy))
                {
                    personsController.ViewData["CurrentSortBy"] = Convert.ToString(sortBy);
                }
                else
                {
                    personsController.ViewData["CurrentSortBy"] = nameof(PersonResponse.PersonName);
                }
                if (parameters.TryGetValue("sortOrder", out object? sortOrder))
                {
                    personsController.ViewData["CurrentSortOrder"] = Convert.ToString(sortOrder);
                } else
                {
                    personsController.ViewData["CurrentSortOrder"] = nameof(SortOrderOptions.ASC);
                }
            }
         
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //To do : add before logic here 
            context.HttpContext.Items["arguments"] = context.ActionArguments;
            _logger.LogInformation("{FilterName}.{MethodName} method",nameof(PersonsListActionFilter),nameof(OnActionExecuting));
            if (context.ActionArguments.TryGetValue("searchBy", out object? value))
            {
                string? searchBy = Convert.ToString(value);
                if (!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.Address),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.ReceiveNewsLetters),
                        nameof(PersonResponse.CountryID)
                    };
                    if(!searchByOptions.Any(temp => temp == searchBy))
                    {
                        _logger.LogInformation("searchBy actual value {searchBy}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        _logger.LogInformation("searchBy updated value {searchBy}", searchBy);
                    }
                }
            }
        }
    }
}
