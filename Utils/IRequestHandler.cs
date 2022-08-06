using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnlightedApiConsumer.Utils
{
    public interface IRequestHandler
    {
        Task<(KeyValuePair<bool, string> response, T? result)> GetRequestResponseAsync<T>(string endpoint, string username, string current_date_string, string apiKey);
    }
}
