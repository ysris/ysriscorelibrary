using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Models.Financial;

namespace YsrisCoreLibrary.Dal.Financial
{
    public class CoinMarketCapDotComDal
    {
        public IEnumerable<CoinMarketCapDotNetTicker> GetTickers()
        {
            var raw =
                new HttpClient()
                .GetAsync($"https://api.coinmarketcap.com/v1/ticker/")
                .Result.Content.ReadAsStringAsync().Result;

            var tickers =
                JsonConvert.DeserializeObject<List<CoinMarketCapDotNetTicker>>(raw)
                .ToList()
                ;

            var usdt = tickers.Single(a => a.symbol == "USDT");
            tickers.Add(new CoinMarketCapDotNetTicker { symbol = "USD", price_btc = usdt.price_btc });

            return tickers;
        }
    }
}
