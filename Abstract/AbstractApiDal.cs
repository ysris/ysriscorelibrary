using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OAuth;

namespace YsrisCoreLibrary.Abstract
{
    ////TODO : Move into proper class file in core
    public abstract class AbstractApiDal
    {
        public T GetJsonWithOAuth1<T>(string uri, string publicKey, string secretKey, Dictionary<string, string> additionalHeaders) where T : class
        {
            var client = OAuthRequest.ForRequestToken(publicKey, secretKey);
            client.RequestUrl = uri;

            // Using HTTP header authorization
            var request = (HttpWebRequest)WebRequest.Create(client.RequestUrl);
            request.Method = "GET";
            request.Headers["Authorization"] = client.GetAuthorizationHeader();

            if (additionalHeaders != null)
                foreach (var cur in additionalHeaders)
                    request.Headers[cur.Key] = cur.Value.ToString();

            string rawJson = new StreamReader(request.GetResponseAsync().Result.GetResponseStream()).ReadToEnd();
            return JsonConvert.DeserializeObject<T>(rawJson);
        }
    }
}
