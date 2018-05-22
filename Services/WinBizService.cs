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

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="configuration"></param>
        public WinBizService(IConfiguration configuration)
        {
            _configuration = configuration;
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
        public WinBizAccountProjectionModel GetProjection(string winbizCustomerId)
        {
            var accounts = ListCustomerAccounts(winbizCustomerId);
            var dic =
                new WinBizAccountProjectionModel
                {
                    dashboard = new WinBizAccountDashboardProjection
                    {
                        salesRevenue = accounts.Between("3000", "3899").Sum(a => a.solde),
                        charges = accounts.Between("3900", "8899").Sum(a => a.solde),
                        preTaxIncome = accounts.Between("3000", "8999").Sum(a => a.solde),
                        cashFlow = accounts.Between("1000", "1099").Sum(a => a.solde),
                        customerCurrencyCode = accounts.Select(a => a.pla_monnai).Distinct().Single(),
                    },
                    balanceSheet = new WinBizAccountBalanceSheetProjection
                    {
                        totalAssets = accounts.Between("1000", "1899").Sum(a => a.solde),
                        currentAssets = accounts.Between("1000", "1399").Sum(a => a.solde),
                        treasury = accounts.Between("1000", "1099").Sum(a => a.solde),
                        clients = accounts.Between("1100", "1139").Sum(a => a.solde),
                        supply = accounts.Between("1200", "1299").Sum(a => a.solde),
                        otherShortTermAssets = accounts.Between("1300", "1399").Concat(accounts.Between("1140", "1199")).Sum(a => a.solde),

                        fixedAsset = accounts.Between("1400", "1899").Sum(a => a.solde),
                        immobilisation = accounts.Between("1400", "1499").Sum(a => a.solde),
                        equipment = accounts.Between("1500", "1699").Sum(a => a.solde),
                        otherImmobilisedAsset = accounts.Between("1700", "1899").Sum(a => a.solde),

                        totalLiabilities = accounts.Between("2000", "2699").Sum(a => a.solde),
                        shortTermDebt = accounts.Between("2000", "2399").Sum(a => a.solde),
                        suppliers = accounts.Between("2000", "2099").Sum(a => a.solde),
                        otherShortTermDebts = accounts.Between("2100", "2399").Sum(a => a.solde),

                        longTermDebt = accounts.Between("2400", "2599").Sum(a => a.solde),
                        provisions = accounts.Between("2600", "2699").Sum(a => a.solde),

                        ownFunds = accounts.Between("2800", "2979").Sum(a => a.solde),
                        capital = accounts.Between("2800", "2899").Sum(a => a.solde),
                        reserve = accounts.Between("2900", "2970").Sum(a => a.solde),
                        pnl = accounts.Between("2979", "2979").Sum(a => a.solde),
                    },
                    incomeStatement = new
                    {
                        salesRevenue = accounts.Between("3000", "3899").Sum(a => a.solde),
                        operatingCosts = accounts.Between("3900", "6899").Sum(a => a.solde),
                        purchases = accounts.Between("3900", "4999").Sum(a => a.solde),
                        personnelExpenses = accounts.Between("5000", "5999").Sum(a => a.solde),
                        otherOperatingCosts = accounts.Between("6000", "6799").Sum(a => a.solde),
                        repayment = accounts.Between("6800", "6899").Sum(a => a.solde),
                        operatingIncome = accounts.Between("3000", "6899").Sum(a => a.solde),
                        financialResults = accounts.Between("6900", "6999").Sum(a => a.solde),
                        financialCharges = accounts.Between("6900", "6949").Sum(a => a.solde),
                        financialProducts = accounts.Between("6950", "6999").Sum(a => a.solde),
                        extraordinaryResult = accounts.Between("7900", "8899").Sum(a => a.solde),
                        preTaxIcome = accounts.Between("3000", "8999").Sum(a => a.solde),
                        incomeOnTaxesAndCapital = accounts.Between("8900", "8999").Sum(a => a.solde),
                        excercisePnl = accounts.Between("3000", "8999").Sum(a => a.solde),

                    }
                    //accounts = accounts,

                };
            return dic;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="winbizCustomerId"></param>
        /// <returns></returns>
        public IEnumerable<WinBizAccount> ListCustomerAccounts(string winbizCustomerId)
        {
            var set =
                apiCall<WinBizResponse<List<WinBizAccount>>>(
                    new { Method = "ChartOfAccounts", Parameters = new List<string> { } }
                    , winbizCustomerId
                )
                .Value
                .ToList()
                ;

            foreach (var item in set)
            {
                item.winbizCustomerId = winbizCustomerId;
                item.valuationDate = DateTime.Now;
            }

            return set;
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
        private HttpClient getHttpClient(string companyId, int? year = null)
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
        public static IEnumerable<WinBizAccount> Between(this IEnumerable<WinBizAccount> collection, string accountFrom, string accountTo)
        {
            return collection.Where(x => x.numero.CompareTo(accountFrom) >= 0 && x.numero.CompareTo(accountTo) <= 0);
        }
    }

    public class WinBizAccountProjectionModel
    {
        public WinBizAccountDashboardProjection dashboard { get; internal set; }
        public WinBizAccountBalanceSheetProjection balanceSheet { get; internal set; }
        public object incomeStatement { get; set; }
    }

    public class WinBizResponse<T>
    {
        public int ErrorsCount { get; set; }
        public string ErrorLast { get; set; }
        public string ErrorsMsg { get; set; }
        public T Value { get; set; }
    }

    public class WinBizAccountDashboardProjection
    {
        public decimal salesRevenue { get; internal set; }
        public decimal charges { get; internal set; }
        public decimal preTaxIncome { get; internal set; }
        public decimal cashFlow { get; internal set; }
        public string customerCurrencyCode { get; internal set; }
        public IEnumerable<WinBizAccount> accounts { get; internal set; }
    }

    public class WinBizAccountBalanceSheetProjection
    {
        public decimal totalAssets { get; internal set; }
        public decimal currentAssets { get; internal set; }
        public decimal treasury { get; internal set; }
        public decimal clients { get; internal set; }
        public decimal supply { get; internal set; }
        public decimal otherShortTermAssets { get; internal set; }
        public decimal fixedAsset { get; internal set; }
        public decimal immobilisation { get; internal set; }
        public decimal equipment { get; internal set; }
        public decimal otherImmobilisedAsset { get; internal set; }
        public decimal otherShortTermDebts { get; internal set; }
        public decimal suppliers { get; internal set; }
        public decimal shortTermDebt { get; internal set; }
        public decimal totalLiabilities { get; internal set; }
        public decimal longTermDebt { get; internal set; }
        public decimal provisions { get; internal set; }
        public decimal ownFunds { get; internal set; }
        public decimal capital { get; internal set; }
        public decimal reserve { get; internal set; }
        public decimal pnl { get; internal set; }
    }

}
