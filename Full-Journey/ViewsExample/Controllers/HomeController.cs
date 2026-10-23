using Microsoft.AspNetCore.Mvc;
using ViewsExample.Models;
namespace ViewsExample.Controllers
{
    public class HomeController : Controller
    {
        [Route("home")]
        [Route("/")]
        public IActionResult Index()
        {
            List<Person> people = new List<Person>()
    {
        new()
        {
            PersonName="John",
            DateOfBirth=DateTime.Parse("2000-05-05"),
            PersonGender=Gender.Male
        },
        new()
        {
            PersonName="Linda",
            DateOfBirth=DateTime.Parse("2004-05-05"),
            PersonGender=Gender.Female
        },
        new()
        {
            PersonName="Susan",
            DateOfBirth=DateTime.Parse("2002-05-05"),
            PersonGender=Gender.Female
        }
    };
            ViewData["appTitle"] ="Asp.Net Core Demo App";
            ViewBag.people = people;

            return View(people);
            //return new ViewResult() { ViewName = "abc" };
        }
        [Route("person-details/{personName}")]
        public IActionResult Details(string? personName)
        {
            if(personName == null)
            {
                return Content("Persn name can't be null");
            }
            List<Person> people = new List<Person>()
    {
        new()
        {
            PersonName="John",
            DateOfBirth=DateTime.Parse("2000-05-05"),
            PersonGender=Gender.Male
        },
        new()
        {
            PersonName="Linda",
            DateOfBirth=DateTime.Parse("2004-05-05"),
            PersonGender=Gender.Female
        },
        new()
        {
            PersonName="Susan",
            DateOfBirth=DateTime.Parse("2002-05-05"),
            PersonGender=Gender.Female
        }
    };
            Person person = people.Where(x => x.PersonName == personName).FirstOrDefault();
            return View(person);

        }
        [Route("person-with-product")]
        public IActionResult PersonWithProduct()
        {
            Person person = new()
            {
                PersonName = "Sara",
                PersonGender = Gender.Female
            ,
                DateOfBirth = Convert.ToDateTime("2004-05-01")
            };

            Product product = new()
            {
                    ProductId=1,
                    ProductName="Air Conditioner"
            };
            return View(new PersonAndProduct()
            {
                PersonData=person,
                ProductData=product
            });
        }
    }
}
