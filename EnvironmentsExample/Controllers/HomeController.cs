using Microsoft.AspNetCore.Mvc;

namespace EnvironmentsExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHost;

        public HomeController(IWebHostEnvironment webHost)
        {
            _webHost = webHost;   
        }
        [Route("/")]
        //[Route("some-route")]
        public IActionResult Index()
        {
            ViewBag.CurrentEnvironment = _webHost.EnvironmentName;
            return View();
        }
      
        //[Route("some-route")]
        //public IActionResult Other()
        //{
        //    return View();
        //}
    }
}
