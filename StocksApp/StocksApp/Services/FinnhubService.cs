using StocksApp.ServiceContracts;
using System.Text.Json;

namespace StocksApp.Services
{
    public class FinnhubService : IFinnhubService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IConfiguration _configuration;

        public FinnhubService(IHttpClientFactory httpClientFactory, IConfiguration configration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configration;
        }

        public async Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
        {
            using (HttpClient httpclient = _httpClientFactory.CreateClient())
            {

                HttpRequestMessage RequestMessage = new HttpRequestMessage()
                {
                    RequestUri = new Uri($"https://finnhub.io/api/v1/quote?symbol={stockSymbol}&token={_configuration["FinnhubToken"]}"),
                    Method = HttpMethod.Get
                };

                HttpResponseMessage responseMessage = await httpclient.SendAsync(RequestMessage);

                Stream stream = await responseMessage.Content.ReadAsStreamAsync();

                StreamReader reader = new StreamReader(stream);
                string response = await reader.ReadToEndAsync();

                //string response1 = await responseMessage.Content.ReadAsStringAsync();

                Dictionary<string, object>? responseDic = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

                if (responseDic == null)
                {
                    throw new InvalidOperationException("No response from finnhub server");
                }

                if (responseDic.ContainsKey("error"))
                {
                    throw new InvalidOperationException(Convert.ToString(responseDic["error"]));
                }
                return responseDic;
            }
        }
    }
}
