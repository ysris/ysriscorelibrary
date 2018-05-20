using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Models.WinBiz;

namespace YsrisCoreLibrary.Services
{
    public class WinBizService
    {
        private IConfiguration _configuration;

        public WinBizService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<WinBizCustomer> ListWinBizCustomers()
        {
            return
                apiCall<WinBizResponse<List<ExpandoObject>>>(new { Method = "Folders", Parameters = new List<string> { } }).Value
                .Select(a => new WinBizCustomer(a))
                ;
        }

        private T apiCall<T>(object entity) where T : class
        {
            var formContent = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            var raw = getHttpClient().PostAsync("https://api.winbizcloud.ch/Bizinfo", formContent).Result.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(raw);
        }

        private HttpClient getHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("winbiz-companyname", _configuration.GetValue<string>("Data:winbiz-companyname"));
            client.DefaultRequestHeaders.Add("winbiz-username", _configuration.GetValue<string>("Data:winbiz-username"));
            client.DefaultRequestHeaders.Add("winbiz-password", Encrypt(_configuration.GetValue<string>("Data:winbiz-password"), _configuration.GetValue<string>("Data:winbiz-publickey")));
            client.DefaultRequestHeaders.Add("winbiz-companyid", "1"); //1:karpeo 7777:testsarl
            client.DefaultRequestHeaders.Add("winbiz-year", "2018");
            client.DefaultRequestHeaders.Add("winbiz-key", _configuration.GetValue<string>("Data:winbiz-key"));
            return client;
        }

        private string Encrypt(string data, string publicKey)
        {
            var cspParams = new CspParameters { ProviderType = 1 };
            var rsaProvider = new RSACryptoServiceProvider(cspParams);
            rsaProvider.ImportCspBlob(Convert.FromBase64String(publicKey));
            var plainBytes = Encoding.UTF8.GetBytes(data);
            var encryptedBytes = rsaProvider.Encrypt(plainBytes, false);
            var encryptedString = Convert.ToBase64String(encryptedBytes);
            return encryptedString;
        }
    }

    public class WinBizResponse<T>
    {
        public int ErrorsCount { get; set; }
        public string ErrorLast { get; set; }
        public string ErrorsMsg { get; set; }
        public T Value { get; set; }
    }

}
