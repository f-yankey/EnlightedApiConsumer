using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnlightedApiConsumer.Utils
{
    public class RequestHandler : IRequestHandler
    {
        //private string _baseUrl;
        private RestClient _client;

        public RequestHandler(string baseUrl)
        {
            //_baseUrl = baseUrl;
            _client = new RestClient(baseUrl);
        }

        public async Task<(KeyValuePair<bool, string> response, T? result)> GetRequestResponseAsync<T>(string endpoint, string username, string current_date_string, string apiKey)
        {
            try
            {
                
                long currentTimeStamp = AuthorizationHandler.GetTimeStamp(current_date_string);
                string authorizationHash = AuthorizationHandler.GetAuthorizationHash(username, apiKey, currentTimeStamp);

                var request = new RestRequest(endpoint);
                request.AddHeader("ApiKey", username);
                request.AddHeader("ts", currentTimeStamp);
                request.AddHeader("Authorization", authorizationHash);

                var res = await _client.GetAsync(request);
                Console.WriteLine(res.Content);
                Console.WriteLine("");

                var response = await _client.GetAsync<T>(request);

                return (new KeyValuePair<bool, string>(true, $"Success!"), response);
            }
            catch (Exception ex)
            {
                return (new KeyValuePair<bool, string>(false, $"An Error Occured! {ex.Message}"), result: default(T));
            }
        }
    }
}
