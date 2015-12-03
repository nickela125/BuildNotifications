using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildNotifications.Interface.Client
{
    public interface IRestClient
    {
        Task<TResponse> PostRequest<TResponse>(IList<KeyValuePair<string, string>> requestValues, string address);
        Task<TResponse> GetRequest<TResponse>(IList<KeyValuePair<string, string>> requestParameters, string address);
        void SetBasicCredentials(string encodedCredentials);
    }
}
