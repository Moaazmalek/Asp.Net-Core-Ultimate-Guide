using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.AuthorizationFilters;
using CRUDExample.Filters.ExceptionFilters;
using CRUDExample.Filters.ResourceFilters;
using CRUDExample.Filters.ResultFilters;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using CRUDExample.Filters;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    //[TypeFilter(typeof(ResponseHeaderActionFilter),Arguments = ["My-Key-From-Controller","My-Value-From-Controller",3],Order =3)]
    [ResponseHeaderFilterFactory("My-Key-From-Controller","My-Value-From-Controller",3)]
    [TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonsAlwaysRunResultFilter))]
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personService;
        private readonly  IPersonsGetterService _personsGetterService;
        private readonly  IPersonsAdderService _personsAdderService;
        private readonly  IPersonsUpdaterService _personsUpdaterService;
        private readonly  IPersonsDeleterService _personsDeleterService;
        private readonly  IPersonsSorterService _personsSorterService ;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(IPersonsGetterService personsGetterService,
            IPersonsAdderService personsAdderService,
            IPersonsUpdaterService personsUpdaterService,
            IPersonsDeleterService personsDeleterService,
            IPersonsSorterService personsSorterService,
            ICountriesService countriesService, 
            ILogger<PersonsController> logger)
        {
            _personsGetterService = personsGetterService;
            _personsAdderService = personsAdderService;
            _personsUpdaterService = personsUpdaterService;
            _personsDeleterService = personsDeleterService;
            _personsSorterService = personsSorterService;
            _countriesService = countriesService;
            _logger = logger;
        }

        [Route("index")]
        [Route("/")]
        [TypeFilter(typeof(PersonsListActionFilter),Order =4)]
        //[TypeFilter(typeof(ResponseHeaderActionFilter),Arguments = new object[]
        //{
        //    "My-Key-From-Action","My-Value-From-Action",1
        //},Order =1)]
        [TypeFilter(typeof(PersonsListResultFilter))]
        [ResponseHeaderFilterFactory("MyKey-FromAction","MyValue-From-Action",1)]
        [SkipFilter]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            _logger.LogInformation("Index action method of PersonsController");
            _logger.LogDebug($"searchBy: {searchBy}, searchString: {searchString}, sortBy: {sortBy}, sortOrder: {sortOrder}");
            List<PersonResponse> persons =await _personsGetterService.GetFilteredPersons(searchBy, searchString);
            //Searching
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                {nameof(Person.PersonName), "Person Name" },
                {nameof(Person.Email), "Email" },
                {nameof(Person.DateOfBirth), "Date of Birth" },
                {nameof(Person.Gender), "Gender" }, 
                {nameof(Person.CountryID), "Country" },
                {nameof(Person.Address), "Address" },
            };

            //ViewBag.CurrentSearchBy = searchBy;
            //ViewBag.CurrentSearchString = searchString;
            //Sorting
            List<PersonResponse> sortedPersons = await _personsSorterService.GetSortedPersons(persons, sortBy, sortOrder);
            //ViewBag.CurrentSortBy = sortBy;
            //ViewBag.CurrentSortOrder = sortOrder.ToString();
            return View(sortedPersons); //Views/Persons/Index.cshtml
        }

        [Route("[action]")]
        [HttpGet]
        [TypeFilter(typeof(ResponseHeaderActionFilter),Arguments =new object[]
        {
            "my-key","my-value",4
        })]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(c => new SelectListItem()
            {
                Text = c.CountryName,
                Value = c.CountryID.ToString(),

            });



            return View();
        }

        [Route("create")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter),Arguments =new object[] {false})]
        public async  Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            PersonResponse personResponse =await _personsAdderService.AddPerson(personRequest);
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")] //Eg: /persons/edit/1
        //[TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? personResponse=await _personsGetterService.GetPersonByPersonID(personID); 
            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }
            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            List<CountryResponse> countries =await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(c => new SelectListItem()
            {
                Text = c.CountryName,
                Value = c.CountryID.ToString()
            });

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? personResponse =await _personsGetterService.GetPersonByPersonID(personRequest.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");
            PersonResponse updatedPerson =await _personsUpdaterService.UpdatePerson(personRequest);
            return RedirectToAction("Index");
            
        }


        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? personResponse =await _personsGetterService.GetPersonByPersonID(personID);
            if (personResponse == null)
                return RedirectToAction("Index");

            return View(personResponse);
        }
         
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateResult)
        {
            PersonResponse? personResponse =await _personsGetterService.GetPersonByPersonID(personUpdateResult.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");

            await _personsDeleterService.DeletePerson(personUpdateResult.PersonID);
            return RedirectToAction("Index");
        }
        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            //Get the list of persons
            List<PersonResponse> persons=await _personsGetterService.GetAllPersons();

            //Return View as PDF
            return new ViewAsPdf("PersonsPDF", persons, ViewData)
            {
                PageMargins=new Margins()
                {
                    Top=20,Right=20,Bottom=20,Left=20
                },
                PageOrientation=Orientation.Landscape
            };
        }
        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
          MemoryStream memoryStream= await _personsGetterService.GetPersonsCSV();
            return File(memoryStream, "application/octet-stream","persons.csv");
        }
    }
}
