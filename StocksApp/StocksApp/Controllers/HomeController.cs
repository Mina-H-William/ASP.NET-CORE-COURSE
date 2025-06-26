using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StocksApp.Models;
using StocksApp.ServiceContracts;
using StocksApp.Services;

namespace StocksApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFinnhubService _finnhibService;

        private readonly TradingOptions _options;

        public HomeController(IFinnhubService finnhibService, IOptions<TradingOptions> options)
        {
            _finnhibService = finnhibService;
            _options = options.Value;
        }

        [Route("/")]
        public async Task<IActionResult> Index()
        {
            Dictionary<string, object>? Dic = await _finnhibService.GetStockPriceQuote(_options.DefaultStockSymbol ?? "MSFT");

            Stock stock = new Stock()
            {
                StockSymbolName = _options.DefaultStockSymbol ?? "MSFT",
                CurrentPrice = Convert.ToDouble(Dic["c"].ToString()),
                LowestPrice = Convert.ToDouble(Dic["l"].ToString()),
                HighestPrice = Convert.ToDouble(Dic["h"].ToString()),
                OpenPrice = Convert.ToDouble(Dic["o"].ToString())
            };

            return View(stock);
        }
    }
}
