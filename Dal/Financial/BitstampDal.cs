//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Numerics;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;

//namespace YsrisCoreLibrary.Dal.Financial
//{
//    public class BitstampDal
//    {
//        private static HMACSHA256 encryptor = new HMACSHA256(Encoding.UTF8.GetBytes(Configuration.bitstampPrivateKey));
//        private static BigInteger CurrentHttpPostNonce { get; set; }
//        private static DateTime DateTimeUnixEpochStart => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

//        // public Dictionary<string, IEnumerable<BitstampTradeHistory>> GetTradeHistory()
//        // {
//        //     var raw = call("user_transactions");
//        //     //Directly deserialize the JObject into dictionary of props
//        //     var obj = JsonConvert.DeserializeObject<List<Dictionary<string, JToken>>>(raw);

//        //     var qry =
//        //         from x in obj
//        //         let u = x.Skip(1).First()
//        //         let instr = u.Key
//        //         let rate = Convert.ToDecimal(u.Value.ToString())

//        //         select new BitstampTradeHistory
//        //         {
//        //             instr = instr,
//        //             rate = (decimal)rate,
//        //             usd = Convert.ToDecimal(x["usd"].ToString()),
//        //             // order_id = (string)x["order_id"],
//        //             datetime = Convert.ToDateTime(x["datetime"].ToString()),
//        //             fee = Convert.ToDecimal(x["fee"].ToString()),
//        //             btc = Convert.ToDecimal(x["btc"].ToString()),
//        //             type = Convert.ToInt32(x["type"].ToString()),
//        //             id = x["id"].ToString(),
//        //             eur = Convert.ToDecimal(x["eur"].ToString()),
//        //         };

//        //     var qry2 =
//        //         qry.GroupBy(a => a.instr)
//        //         .ToDictionary(a => a.Key, a => a.AsEnumerable());
//        //     return qry2;
//        // }

//        public Dictionary<string, decimal> GetBalances()
//        {
//            var raw = call("balance");
//            var obj = JsonConvert.DeserializeObject<dynamic>(raw);
//            var dic = new Dictionary<string, decimal> {
//                {"BTC", Convert.ToDecimal(obj.btc_balance.ToString()) },
//                {"EUR", Convert.ToDecimal(obj.eur_balance.ToString()) },
//                {"USD", Convert.ToDecimal(obj.usd_balance.ToString()) },
//                {"LTC", Convert.ToDecimal(obj.ltc_balance.ToString()) },
//                {"XRP", Convert.ToDecimal(obj.xrp_balance.ToString()) },
//            };
//            return dic.OrderBy(a => a.Key).ToDictionary(a => a.Key, a => a.Value); ;
//        }

//        private string call(string command, Dictionary<string, string> parameters = null)
//        {
//            string nonce = getCurrentHttpPostNonce();
//            var signature =
//                encryptor.ComputeHash(Encoding.UTF8.GetBytes(nonce + Configuration.bistampUserId + Configuration.bistampPublicKey))
//                .ToStringHex().ToUpper();

//            var dico = new Dictionary<string, string>
//            {
//                {"key", Configuration.bistampPublicKey},
//                {"signature", signature},
//                {"nonce", nonce}
//            };
//            foreach (var cur in parameters ?? new Dictionary<string, string>())
//                dico[cur.Key] = cur.Value;

//            var content = new FormUrlEncodedContent(dico);
//            var raw = new HttpClient().PostAsync($"https://www.bitstamp.net/api/v2/{command}/", content).Result.Content.ReadAsStringAsync().Result;
//            return raw;
//        }

//        private string getCurrentHttpPostNonce()
//        {
//            var newHttpPostNonce = new BigInteger(Math.Round(DateTime.UtcNow.Subtract(DateTimeUnixEpochStart).TotalMilliseconds * 1000, MidpointRounding.AwayFromZero));
//            if (newHttpPostNonce > CurrentHttpPostNonce)
//                CurrentHttpPostNonce = newHttpPostNonce;
//            else
//                CurrentHttpPostNonce += 1;
//            return CurrentHttpPostNonce.ToString();
//        }
//    }
//}
