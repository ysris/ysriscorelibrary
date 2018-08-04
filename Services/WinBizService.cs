using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ysriscorelibrary.Interfaces;
using YsrisCoreLibrary.Models.WinBiz;

namespace YsrisCoreLibrary.Services
{
    public class WinBizService
    {
        private readonly IConfiguration _configuration;
        private readonly IStorageService _storage;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="configuration"></param>
        public WinBizService(IConfiguration configuration, IStorageService storage)
        {
            _configuration = configuration;
            _storage = storage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WinBizCustomer> ListWinBizCustomers()
        {
            return
                apiCall<WinBizResponse<List<ExpandoObject>>>(
                    new { Method = "Folders", Parameters = new List<string>() },
                    _configuration.GetValue<string>("Data:winbiz-rootcompanyid")
                ).Value
                .Select(a => new WinBizCustomer(a))
                ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="winbizCustomerId"></param>
        /// <returns></returns>
        public IEnumerable<WinBizHistoricalAccount> ListCustomerAccounts(string winbizCustomerId)
        {
            var set = apiCall<WinBizResponse<List<WinBizHistoricalAccount>>>(new { Method = "ChartOfAccounts", Parameters = new List<string> { } }, winbizCustomerId).Value.ToList();

            foreach (var item in set)
            {
                item.winbizCustomerId = winbizCustomerId;
                item.valuationDate = DateTime.Now;
            }

            return set;
        }


        public WinBizHistoricalRow GetDailyRow(string winbizCustomerId)
        {
            var accounts = ListCustomerAccounts(winbizCustomerId);
            var entity = new WinBizHistoricalRow
            {
                winbizClientdos_numero = winbizCustomerId,
                valuationDate = DateTime.Now, //intraday datetime
                salesRevenue = -1 * accounts.Between("3000", "3899").Sum(a => a.solde),
                charges = accounts.Between("3900", "8899").Sum(a => a.solde),
                preTaxIncome = -1 * accounts.Between("3000", "8999").Sum(a => a.solde),
                cashFlow = accounts.Between("1000", "1030").Sum(a => a.solde),
                customerCurrencyCode = accounts.Select(a => a.pla_monnai).Distinct().Single(),

                activityEvolution = accounts.Between("1000", "1030").Sum(a => a.solde),
                resultEvolution = accounts.Between("8902", "9000").Sum(a => a.solde),
                //treasuryEvolution = accounts.Between("1000", "1030"),
            };

            return entity;
        }

        public IEnumerable<WinBizHistoricalRow> GetDailyRow(IEnumerable<string> customersIds)
        {
            return customersIds.Select(a => GetDailyRow(a));
        }




        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="companyId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private T apiCall<T>(object entity, string companyId, int? year = null) where T : class
        {
            var formContent = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            var raw = getHttpClient(companyId, year).PostAsync("https://api.winbizcloud.ch/Bizinfo", formContent).Result.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(raw);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public HttpClient getHttpClient(string companyId, int? year = null)
        {
            if (year == null)
                year = DateTime.Now.Year;

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("winbiz-companyname", _configuration.GetValue<string>("Data:winbiz-companyname"));
            client.DefaultRequestHeaders.Add("winbiz-username", _configuration.GetValue<string>("Data:winbiz-username"));
            client.DefaultRequestHeaders.Add("winbiz-password", encrypt(_configuration.GetValue<string>("Data:winbiz-password"), _configuration.GetValue<string>("Data:winbiz-publickey")));
            client.DefaultRequestHeaders.Add("winbiz-companyid", companyId); //1:karpeo 7777:testsarl
            client.DefaultRequestHeaders.Add("winbiz-year", year.ToString());
            client.DefaultRequestHeaders.Add("winbiz-key", _configuration.GetValue<string>("Data:winbiz-key"));
            return client;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        private string encrypt(string data, string publicKey)
        {
            var rsaProvider = new RSACryptoServiceProvider();
            rsaProvider.ImportCspBlob(Convert.FromBase64String(publicKey));
            var plainBytes = Encoding.UTF8.GetBytes(data);
            var encryptedBytes = rsaProvider.Encrypt(plainBytes, false);
            var encryptedString = Convert.ToBase64String(encryptedBytes);
            return encryptedString;
        }

        public void UploadCsv(string item, string companyId)
        {
            var url = "https://api.winbizcloud.ch/Bizinfo";
            var filePath = _storage.GetFullPath(item);
            var content = new ByteArrayContent(File.ReadAllBytes(filePath));
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");

            var form = new MultipartFormDataContent();
            form.Add(new StringContent("ENTRIESIMPORT"), "winbiz-method");
            form.Add(content, "winbiz-file", Path.GetFileName(filePath));

            var httpClient = getHttpClient(companyId);
            var response = httpClient.PostAsync(url, form).Result;

            
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class WinBizServiceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="accountFrom"></param>
        /// <param name="accountTo"></param>
        /// <returns></returns>
        public static IEnumerable<WinBizHistoricalAccount> Between(this IEnumerable<WinBizHistoricalAccount> collection, string accountFrom, string accountTo)
        {
            return collection.Where(x => x.numero.CompareTo(accountFrom) >= 0 && x.numero.CompareTo(accountTo) <= 0);
        }

        public static IEnumerable<WinBizHistoricalAccount> Any(this IEnumerable<WinBizHistoricalAccount> collection, IEnumerable<string> accounts)
        {
            return collection.Where(x => accounts.Contains(x.numero));
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
