
using ContactsManager.Core.DTO;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.UI.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace CRUDExample.Filters.ActionFilters
{
    public class PersonCreateAndEditPostActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonCreateAndEditPostActionFilter> _logger;

        public PersonCreateAndEditPostActionFilter(ICountriesService countriesService, ILogger<PersonCreateAndEditPostActionFilter> logger)
        {
            _countriesService = countriesService;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //before logic
            if(context.Controller is PersonsController personsController)
            {
                if (!context.ModelState.IsValid)
                            {
                                List<CountryResponse> countries = await _countriesService.GetAllCountries(); 
                                personsController.ViewBag.Countries = countries.Select(c => new SelectListItem()
                                {
                                    Text = c.CountryName,
                                    Value = c.CountryID.ToString()
                                });
                                personsController.ViewBag.Errors = personsController.ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();
                    //var personRequest =context.ActionArguments.Values.FirstOrDefault(arg => arg is PersonAddRequest || arg is PersonUpdateRequest);
                    var personRequest = context.ActionArguments["personRequest"];
                    context.Result = personsController.View(personRequest);//short-circuits the action the subsequents filters & action methods

                             }
                    else
                                {
                                    await next();
                                                //after logic 
                                }
            }else
            {
                await next();
            }

            _logger.LogInformation("In after logic of PersonsCreateAndEditPost Action Filter");
        }
    }
}
