using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using BuildNotifications.Interface.Client;
using BuildNotifications.Model;
using Newtonsoft.Json;

namespace BuildNotifications.Client
{
    public class RestClient : IRestClient
    {
        private string _encodedCredentials;

        public async Task<TResponse> PostRequest<TResponse>(IList<KeyValuePair<string, string>> requestValues, string address)
        {
            using (HttpClient client = GetClient())
            {
                var content = new FormUrlEncodedContent(requestValues);
                var result = client.PostAsync(address, content).Result;
                string resultContent = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(resultContent);
            }
        }

        public async Task<TResponse> GetRequest<TResponse>(IList<KeyValuePair<string, string>> requestParameters, string address)
        {
            using (HttpClient client = GetClient())
            {
                string queryString = string.Empty;
                if (requestParameters != null)
                {
                    GetQueryStringParameters(requestParameters);
                }
                HttpResponseMessage result = await client.GetAsync(address + queryString);
                string resultContent = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(resultContent, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            }
        }

        public void SetBasicCredentials(string encodedCredentials)
        {
            _encodedCredentials = encodedCredentials;
        }

        private HttpClient GetClient()
        {
            var client = new HttpClient();
            if (_encodedCredentials != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _encodedCredentials);
            }

            return client;
        }

        private string GetQueryStringParameters(IList<KeyValuePair<string, string>> parameters)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            foreach (KeyValuePair<string, string> requestValue in parameters)
            {
                query[requestValue.Key] = requestValue.Value;
            }
            return query.ToString();
        }
    }
}
