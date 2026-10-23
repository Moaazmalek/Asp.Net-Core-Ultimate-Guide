using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace IActionResultExample.Controllers
{
    public class StoreController : Controller
    {
        [Route("store/books")]
        public IActionResult Books()
        {
            return Content($"Book Found");  
        }
    }
}
