using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ysriscorelibrary.Interfaces;
using YsrisCoreLibrary.Models;
using YsrisCoreLibrary.Models.Yooz;

namespace YsrisCoreLibrary.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class YoozService
    {
        private ILogger<YoozService> _logger;
        private IStorageService _storageService;
        private IConfiguration _configuration;

        public YoozService(
            ILogger<YoozService> logger,
            IStorageService storageService,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _storageService = storageService;
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public void CallYoozUpdater()
        {
            _logger.LogInformation($"+_callYoozUpdater");
            var updaterJarPath = _configuration.GetValue<string>("Data:YoozUpdaterFullPath");
            var cmd = $"sudo java -jar {updaterJarPath}";
            _logger.LogInformation($"+_callYoozUpdater cmd : {cmd}");
            var result = YsrisCoreLibrary.Helpers.ShellHelper.Bash(cmd);
            _logger.LogInformation(result);
            _logger.LogInformation($"-_callYoozUpdater");
        }

        /// <summary>
        /// Upload a document to Yooz service
        /// </summary>
        /// <param name="filePath">filepath (formatted as IStorageService needs it)</param>
        /// <param name="documentTypeKey">type of document to send</param>
        /// <param name="yoozCustomerId">the yooz customer id to upload the document for</param>
        /// <returns></returns>
        public YoozUploadDocumentReturnEntity UploadDocument(string filePath, string documentTypeKey, YoozCustomer yoozCustomer)
        {
            var content = _storageService.GetFileContent(filePath).Result.ToArray();
            var requestContent = new MultipartFormDataContent();
            var content2 = new ByteArrayContent(content);
            content2.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            requestContent.Add(content2, "file", Path.GetFileName(filePath));
            var response = getHttpClient().PostAsync($"documents?ou={yoozCustomer.index}&doctype={documentTypeKey}", requestContent).Result;

            var content3 = response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<IEnumerable<YoozUploadDocumentReturnEntity>>(content3).SingleOrDefault();
        }

        public IEnumerable<YoozCustomer> ListYoozCustomers()
        {
            var page = getHttpClient().GetAsync($"corporates").Result;
            var content = page.Content.ReadAsStringAsync().Result;
            var items = JsonConvert.DeserializeObject<IEnumerable<YoozCustomer>>(content);
            return items;
        }

        private HttpClient getHttpClient()
        {
            var login = _configuration.GetValue<string>("Data:YoozLogin");
            var password = _configuration.GetValue<string>("Data:YoozPassword");

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://www3.yooz.fr/karpeo/api/service/1/");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Base64Encode($"{login}:{password}"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

    }

    public class YoozUploadDocumentReturnEntity
    {
        public int index { get; set; }
        //public string organizationalUnit { get; set; }
        //public string uri { get; set; }
    }
}
