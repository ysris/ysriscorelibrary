using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Services
{
    public class SlackService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IConfiguration _configuration;

        private string _webhookUrl => _configuration.GetValue<string>("Data:SlackHookUrl");

        public SlackService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<HttpResponseMessage> SendMessageAsync(object payload)
        {
            var serializedPayload = JsonConvert.SerializeObject(payload);
            var response = await _httpClient.PostAsync(_webhookUrl, new StringContent(serializedPayload, Encoding.UTF8, "application/json"));
            return response;
        }

        public async Task<HttpResponseMessage> SendSuccessInformationMessage(string title)
        {
            return await SendMessageAsync(new
            {
                title,
                color = "#38BC00",
                //pretext= "One ",
                //author_name = "Karpeo Bot",
                //author_link = "http://karpeo.ysris.ch",
                //author_icon = "https://karpeo.ysris.ch/assets/images/logo-default.png",
                //title_link = "http://karpeo.ysris.ch/doc/___",
            });
        }
    }
}
