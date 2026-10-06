using Entities;
using Entities.DTO;
using Entities.ServiceContracts;
using Entities.ServiceContracts.Enums;
using Entities.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            List<PersonResponse> persons = _personService.GetFilteredPersons(searchBy, searchString);
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
            List<PersonResponse> sortedPersons = _personService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();
            return View(sortedPersons); //Views/Persons/Index.cshtml
        }

        [Route("create")]
        [HttpGet]
        public IActionResult Create()
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(c => new SelectListItem()
            {
                Text = c.CountryName,
                Value = c.CountryID.ToString(),

            });



            return View();
        }

        [Route("create")]
        [HttpPost]
        public IActionResult Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Text = c.CountryName,
                    Value = c.CountryID.ToString()
                });
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }
            PersonResponse personResponse = _personService.AddPerson(personAddRequest);

            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")] //Eg: /persons/edit/1
        public IActionResult Edit(Guid personID)
        {
            PersonResponse? personResponse=_personService.GetPersonByPersonID(personID); 
            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }
            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(c => new SelectListItem()
            {
                Text = c.CountryName,
                Value = c.CountryID.ToString()
            });

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public IActionResult Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = _personService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                PersonResponse updatedPerson = _personService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");

            }
            else
            {
                List<CountryResponse> countries = _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Text = c.CountryName,
                    Value = c.CountryID.ToString()
                });
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return View();
            }
            
        }


        [HttpGet]
        [Route("[action]/{personID}")]
        public IActionResult Delete(Guid? personID)
        {
            PersonResponse? personResponse = _personService.GetPersonByPersonID(personID);
            if (personResponse == null)
                return RedirectToAction("Index");

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public IActionResult Delete(PersonUpdateRequest personUpdateResult)
        {
            PersonResponse? personResponse = _personService.GetPersonByPersonID(personUpdateResult.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");

            _personService.DeletePerson(personUpdateResult.PersonID);
            return RedirectToAction("Index");
        }
    }
}
