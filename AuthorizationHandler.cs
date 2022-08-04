using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EnlightedApiConsumer
{
    public static class AuthorizationHandler
    {
        public static string GetAuthorizationHash(string username, string apiKey, long currentTimeStamp)
        {
            string hash = string.Empty;

            using (SHA1 sha1Hash = SHA1.Create())
            {
                string source = $"{username}{apiKey}{currentTimeStamp}";
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty).ToLower();
            }
            return hash;
        }

        public static long GetTimeStamp(string date_string)
        {
            var date = DateTime.Parse(date_string);
            return new DateTimeOffset(date).ToUnixTimeMilliseconds();
        }

    }
}
