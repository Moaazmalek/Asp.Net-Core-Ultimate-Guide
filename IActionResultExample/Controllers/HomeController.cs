using Microsoft.AspNetCore.Mvc;
using IActionResultExample.Models;
namespace IActionResultExample.Controllers
{
    public class HomeController : Controller
    {
        //[Route("bookstore")]
        //public IActionResult Index(int? bookid)
        //{
        //    //if (!Request.Query.ContainsKey("bookid"))
        //    //{
        //    //    Response.StatusCode = 400;
        //    //    return Content("Book id is not supplied");
        //    //}
        //    if (bookid.HasValue ==false) {
        //        Response.StatusCode = 400;
        //        return Content("Book id is not supplied");
        //    }
        //    // book id can't be empty
        //    if (String.IsNullOrEmpty(Convert.ToString(Request.Query["bookid"])))
        //    {
        //        Response.StatusCode = 400;
        //        return Content("Book id can't be null or empty");
        //    }
        //    // book id should be between 1 to 1000
        //    int bookId = Convert.ToInt32(ControllerContext.HttpContext.Request.Query["bookid"]);
        //    if (bookId <= 0 || bookId > 1000)
        //    {
        //        Response.StatusCode = 400;
        //        return Content("Book id can't be less than 0 or more than 1000");
        //    }
        //    if (Convert.ToBoolean(Request.Query["isloggedin"]) == false)
        //    {
        //        Response.StatusCode = 401;
        //        return Content("User must be authenticated");
        //    }


        //    return new RedirectToActionResult("Books", "Store",new {});
        //}
        [Route("bookstore/{bookid?}/{isloggedin?}")]
        public IActionResult Index([FromRoute]int? bookid, [FromRoute] bool? 
           isloggedin, Book book)
        {
            if (!bookid.HasValue)
            {
                return BadRequest("Book id is not supplied");
            }

            if (bookid <= 0 || bookid > 1000)
            {
                return BadRequest("Book id must be between 1 and 1000");
            }

            if (!Request.Query.TryGetValue("isloggedin", out var isLoggedIn) ||
                !bool.TryParse(isLoggedIn, out var loggedIn) ||
                !loggedIn)
            {
                return Unauthorized("User must be authenticated");
            }

            //return RedirectToAction("Books", "Store");
            return Content($"Book id: {bookid}, Book: {book}", "text/plain");
        }
    }
}
