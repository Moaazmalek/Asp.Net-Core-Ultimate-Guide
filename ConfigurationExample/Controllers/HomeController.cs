using ConfigurationExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ConfigurationExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly WeatherApiOptions _options;
        private readonly IConfiguration _configuration;
        //public HomeController(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}
        public HomeController(IOptions<WeatherApiOptions> options)
        {
            _options = options.Value;
        }
        [Route("/")]
        public IActionResult Index()
        {
            //WeatherApiOptions options =
            //    _configuration.GetSection("weatherapi").Get<WeatherApiOptions>();
            //WeatherApiOptions opt = new();
            //_configuration.GetSection("weatherapi").Bind(opt);
            //ViewBag.ClientId=_configuration["weatherapi:ClientID"];
            //ViewBag.ClientSecret=_configuration.GetValue<string>("weatherapi:ClientSecret");
            //ViewBag.ClientID = _configuration.GetSection("weatherapi")["ClientID"];
            //ViewBag.ClientSecret = _configuration.GetSection("weatherapi")["ClientSecret"];
            ViewBag.ClientID = _options.ClientID;
            ViewBag.ClientSecret = _options.ClientSecret;

            return View();
        }
    }
}
