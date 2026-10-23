using Microsoft.AspNetCore.Mvc;
using ModelValidationsExample.CustomModelBinders;
using ModelValidationsExample.Models;

namespace ModelValidationsExample.Controllers
{
    public class HomeController : Controller
    {
        [Route("register")]
        public IActionResult Index(Person person,[FromHeader(Name ="User-Agent")] string UserAgent)
        {
            if (!ModelState.IsValid)
            {
                string errors= string.Join("\n", ModelState.Values.
                    SelectMany(value => value.Errors).
                    Select(err => err.ErrorMessage));
                //foreach(var value in ModelState.Values)
                //{
                //    foreach(var error in value.Errors)
                //    {
                //        errorsList.Add(error.ErrorMessage);
                //    }
                //}
                //string errors =string.Join("\n",errorsList);
                
                return BadRequest(errors);
            }
            return Content($"person: {person}");
        }
    }
}
