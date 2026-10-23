using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StocksApp.Models;
using StocksApp.ServiceContracts;
using StocksApp.Services;


namespace StocksApp.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly IOptions<TradingOptions> _options;
        private readonly IFinnhubService _finnhubService;
        public HomeController(IFinnhubService finnhubService,
            IOptions<TradingOptions> options)
        {
            _finnhubService = finnhubService;
            _options = options;
        }
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            if(_options.Value.DefaultStockSymbol == null)
            {
                _options.Value.DefaultStockSymbol = "MSFT";
            }
            Dictionary<string,object>? responseDictionary=await _finnhubService.GetStockPriceQuote(stockSymbol:_options.Value.DefaultStockSymbol);
            Stock stock = new()
            {
                StockSymbol = _options.Value.DefaultStockSymbol,
                CurrentPrice = Convert.ToDouble(responseDictionary["c"].ToString()),
                LowestPrice = Convert.ToDouble(responseDictionary["l"].ToString()),
                HighPrice = Convert.ToDouble(responseDictionary["h"].ToString()),
                OpenPrice = Convert.ToDouble(responseDictionary["o"].ToString())

            };


            return View(stock);
        }
    }
}
