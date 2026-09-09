using Microsoft.AspNetCore.Mvc;
using ControllersExample.Models;

namespace ControllersExample.Controllers
{
    [Controller]
    public class HomeController : Controller
    {
        [Route("home")] // attribute routing
        [Route("/")]
        public ContentResult Index()
        {
            //return new ContentResult()
            //{ 
            //    Content = "Hello from Index",
            //    ContentType = "text/plain"
            //};
            //return Content("Hello From Index","text/plain");
            return Content("" +
                "<h1>Welcome </h1> " +
                "<h2>Hello From Index</h2>",
                "text/html");
        }
        [Route("about")] // attribute routing
        public string About()
        {
            return "Hello from About";
        }
        [Route("contact-us/{mobile:regex(^\\d{{10}}$)}")] // attribute routing
        public string Contact(string mobile)
        {
            return $"Hello from Contact {mobile}";
        }
        [Route("person")]
        public JsonResult Person()
        {
            Person person=new Person()
            {
                Id=Guid.NewGuid(),
                FirstName="muath",
                LastName="Al-Farwan",
                Age=22
            };
            return new JsonResult(person);
        }

    }
}
