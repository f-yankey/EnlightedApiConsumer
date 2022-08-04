using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnlightedApiConsumer
{
    public interface IRequestHandler
    {
        Task<(KeyValuePair<bool, string> response, T? result)> GetRequestResponseAsync<T>(string endpoint, string username, long currentTimeStamp, string authorizationHash);
    }
}
