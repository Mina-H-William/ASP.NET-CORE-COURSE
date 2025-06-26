namespace StocksApp.Models
{
    public class Stock
    {
        public string? StockSymbolName { get; set; }

        public double CurrentPrice { get; set; }

        public double LowestPrice { get; set; }

        public double HighestPrice { get; set; }

        public double OpenPrice { get; set; }

    }
}
