using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

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
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            List<PersonResponse> persons =await _personService.GetFilteredPersons(searchBy, searchString);
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
            List<PersonResponse> sortedPersons = await _personService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();
            return View(sortedPersons); //Views/Persons/Index.cshtml
        }

        [Route("create")]
        [HttpGet]
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
        public async  Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
            List<CountryResponse> countries =await _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Text = c.CountryName,
                    Value = c.CountryID.ToString()
                });
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personAddRequest);
            }
            PersonResponse personResponse =await _personService.AddPerson(personAddRequest);

            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")] //Eg: /persons/edit/1
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? personResponse=await _personService.GetPersonByPersonID(personID); 
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
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse =await _personService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                PersonResponse updatedPerson =await _personService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");

            }
            else
            {
                List<CountryResponse> countries =await _countriesService.GetAllCountries();
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
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? personResponse =await _personService.GetPersonByPersonID(personID);
            if (personResponse == null)
                return RedirectToAction("Index");

            return View(personResponse);
        }
         
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateResult)
        {
            PersonResponse? personResponse =await _personService.GetPersonByPersonID(personUpdateResult.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");

            await _personService.DeletePerson(personUpdateResult.PersonID);
            return RedirectToAction("Index");
        }
        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            //Get the list of persons
            List<PersonResponse> persons=await _personService.GetAllPersons();

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
          MemoryStream memoryStream= await _personService.GetPersonsCSV();
            return File(memoryStream, "application/octet-stream","persons.csv");
        }
    }
}
