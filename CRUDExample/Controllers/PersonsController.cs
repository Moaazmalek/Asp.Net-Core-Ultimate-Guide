using Entities;
using Entities.DTO;
using Entities.ServiceContracts;
using Entities.ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CRUDExample.Controllers
{
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personService;
        public readonly ICountriesService _countriesService;
        public PersonsController(IPersonsService personsService, ICountriesService countriesService)
        {
            _personService = personsService;
            _countriesService = countriesService;
        }
        [Route("index")]
        [Route("/")]
        public IActionResult Index(string searchBy,string? searchString, string sortBy=nameof(PersonResponse.PersonName),SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
           List<PersonResponse> persons= _personService.GetFilteredPersons(searchBy,searchString);
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

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;
            //Sorting
          List<PersonResponse> sortedPersons=  _personService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();
            return View(sortedPersons); //Views/Persons/Index.cshtml
        }

        [Route("create")]
        [HttpGet]
        public IActionResult Create()
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries;


            return View();
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(PersonAddRequest personAddRequest)
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            if (!ModelState.IsValid)
            {
                ViewBag.Countries = countries;
               ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }
           PersonResponse personResponse= _personService.AddPerson(personAddRequest);

            return RedirectToAction("Index","Persons");
        }
    }
}
