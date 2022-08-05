using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnlightedApiConsumer
{
    public class RequestHandler : IRequestHandler
    {
        private string _baseUrl;

        public RequestHandler(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<(KeyValuePair<bool, string> response, T? result)> GetRequestResponseAsync<T>(string endpoint, string username, long currentTimeStamp, string authorizationHash)
        {
            try
            {
                var client = new RestClient(_baseUrl);

                var request = new RestRequest(endpoint);
                request.AddHeader("ApiKey", username);
                request.AddHeader("ts", currentTimeStamp);
                request.AddHeader("Authorization", authorizationHash);

                var res = await client.GetAsync(request);
                Console.WriteLine(res.Content);
                Console.WriteLine("");

                var response = await client.GetAsync<T>(request);

                return (new KeyValuePair<bool, string>(true, $"Success!"), response);
            }
            catch (Exception ex)
            {
                return (new KeyValuePair<bool, string>(false, $"An Error Occured! {ex.Message}"), result: default(T));
            }
        }
    }
}
