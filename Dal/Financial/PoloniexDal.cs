using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Classes;
using YsrisCoreLibrary.Models.Financial;

namespace YsrisCoreLibrary.Dal.Financial
{
    public class PoloniexDal
    {
        public static string poloniexPublicKey => "IVCSYDB6-1QT1BTCX-41TFHYBN-PPEI2XXA";
        public static string poloniexPrivateKey => "5c2ccb4565c5db46f2cd208e3bcaa4dc3b67924c217890c65b4563b517551d38207a73d1c437b23d0bc14c686bef44eb0957b632e5d10041bbc0fb6e312f556d";

        private static HMACSHA512 encryptor = new HMACSHA512(Encoding.UTF8.GetBytes(poloniexPrivateKey));
        private static BigInteger CurrentHttpPostNonce { get; set; }
        private static DateTime DateTimeUnixEpochStart => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public IEnumerable<TimeSerieItem> getChartData(string ccyPair, int period)
        {
            try
            {
                var raw =
                    callGet("returnChartData", new Dictionary<string, string> {
                        {"currencyPair",ccyPair}
                         ,{"start","0".ToString()}
                         ,{"end","9999999999".ToString()}
                         ,{"period",period.ToString()}
                    });
                var obj =
                    JsonConvert.DeserializeObject<IEnumerable<PoloniexTimeSerieItemRaw>>(raw)
                        ?? Enumerable.Empty<PoloniexTimeSerieItemRaw>();

                var set = obj.Select(a => new TimeSerieItem(a) { Instrument = ccyPair, Market = "Poloniex", Period = period }).ToList();

                return set;
            }
            catch (JsonSerializationException e)
            {
                return Enumerable.Empty<TimeSerieItem>();
            }
        }



        public Dictionary<string, IEnumerable<PoloniexTradeHistory>> GetTradeHistory()
        {
            var raw = call("returnTradeHistory", new Dictionary<string, string> {
                {"currencyPair","all"}
                 ,{"start","0".ToString()}
            });
            var obj = JsonConvert.DeserializeObject<Dictionary<string, IEnumerable<PoloniexTradeHistory>>>(raw);
            return obj;
        }

        public List<PoloniexTicker> GetTickers()
        {
            var raw = callGet("returnTicker");
            var tickers =
                JsonConvert.DeserializeObject<Dictionary<string, PoloniexTicker>>(raw)
                .Select(a =>
                {
                    a.Value.symbol = a.Key;
                    return a.Value;
                })
                .ToList()
                ;

            tickers.Add(new PoloniexTicker { symbol = "BTC_BTC", last = 1, high24hr = 1, low24hr = 1 });
            var usdtbtc = tickers.Single(a => a.symbol == "USDT_BTC");
            var btcusdt = new PoloniexTicker { symbol = "BTC_USDT", last = 1 / usdtbtc.last, high24hr = 1 / usdtbtc.high24hr, low24hr = 1 / usdtbtc.low24hr };
            tickers.Add(btcusdt);

            return tickers;
        }

        public Dictionary<string, decimal> GetBalances()
        {
            var raw = call("returnBalances");
            var obj = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(raw);
            return obj.Where(a => a.Value > 0).ToDictionary(a => a.Key, a => a.Value);
        }

        private string call(string command, Dictionary<string, string> parameters = null)
        {
            var dico = new Dictionary<string, string> { { "command", command }, { "nonce", getCurrentHttpPostNonce() } };
            foreach (var cur in parameters ?? new Dictionary<string, string>())
                dico[cur.Key] = cur.Value;

            var content = new FormUrlEncodedContent(dico);
            content.Headers.Add("Key", poloniexPublicKey);
            content.Headers.Add("Sign", encryptor.ComputeHash(Encoding.UTF8.GetBytes(dico.ToHttpPostString())).ToStringHex());

            var client = new HttpClient();
            var raw = client.PostAsync("https://poloniex.com/tradingApi", content).Result.Content.ReadAsStringAsync().Result;

            return raw;
        }

        private string callGet(string command, Dictionary<string, string> parameters = null)
        {
            var client = new HttpClient();

            foreach (var cur in parameters ?? new Dictionary<string, string>())
                command += $"&{cur.Key}={cur.Value}";

            var raw = client.GetAsync($"https://poloniex.com/public?command={command}").Result.Content.ReadAsStringAsync().Result;
            return raw;
        }

        private string getCurrentHttpPostNonce()
        {
            var newHttpPostNonce = new BigInteger(Math.Round(DateTime.UtcNow.Subtract(DateTimeUnixEpochStart).TotalMilliseconds * 1000, MidpointRounding.AwayFromZero));
            if (newHttpPostNonce > CurrentHttpPostNonce)
                CurrentHttpPostNonce = newHttpPostNonce;
            else
                CurrentHttpPostNonce += 1;

            return CurrentHttpPostNonce.ToString();
        }
    }

    public static class PoloniexDalExtensions
    {
        public static Dictionary<string, decimal> GetAvgBuyPrice(this Dictionary<string, IEnumerable<PoloniexTradeHistory>> set)
        {
            var qry =
                (from x in set
                 let sub = x.Value.Where(a => a.type == "buy")
                 where sub.Any()
                 let avg = sub.Sum(a => a.total) / sub.Sum(a => a.amount)
                 select new { x.Key, avg })
                 .ToDictionary(a => a.Key, a => a.avg);
            return qry;
        }
    }

    public static class MyExtensions
    {
        public static string ToHttpPostString(this Dictionary<string, string> dictionary)
        {
            var output = string.Empty;
            foreach (var entry in dictionary)
            {
                var valueString = entry.Value as string;
                if (valueString == null)
                {
                    output += "&" + entry.Key + "=" + entry.Value;
                }
                else
                {
                    output += "&" + entry.Key + "=" + valueString.Replace(' ', '+');
                }
            }

            return output.Substring(1);
        }

        public static string ToStringHex(this byte[] value)
        {
            var output = string.Empty;
            for (var i = 0; i < value.Length; i++)
            {
                output += value[i].ToString("x2");
            }

            return (output);
        }



    }
}
