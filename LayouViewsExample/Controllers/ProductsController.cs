using Microsoft.AspNetCore.Mvc;

namespace LayouViewsExample.Controllers
{
    public class ProductsController : Controller
    {
        [Route("products")]
        public IActionResult Index()
        {
            return View();
        }
        //Url : /search-producs/1
        [Route("search-products/{ProductID?}")]
        public IActionResult Search(int? ProductID)
        {
            ViewBag.ProductID = ProductID;
            return View();
        }
        [Route("order-products")]
        public IActionResult Order()
        {
            return View();
        }
    }
}
