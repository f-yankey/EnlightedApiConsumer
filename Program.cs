using EnlightedApiConsumer.Data.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EnlightedApiConsumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Initializing Configurations...");
            IConfiguration configuration = InitializeConfig();

            string _baseUrl = configuration["BaseUrl"];
            string floorsEndPoint = configuration["FloorsEndPoint"];
            string fixtureEndPoint = configuration["FixtureEndPoint"];

            IRequestHandler requestHandler = new RequestHandler(_baseUrl);

            /*Test settings for BOB's example*/
            //string date_string = "3/3/2016 7:36:51.032 PM";
            //string username = "bob";
            //string apiKey = "6eb6f07fd09b18dd61dd353dfb669820e7859cd3";

            /*Live settings*/
            string date_string = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
            string username = configuration["Username"];
            string apiKey = configuration["ApiKey"];

            long currentTimeStamp = AuthorizationHandler.GetTimeStamp(date_string);
            string authorizationHash = AuthorizationHandler.GetAuthorizationHash(username, apiKey, currentTimeStamp);

            //Console.WriteLine($"The current timestamp is {currentTimeStamp}");
            //Console.WriteLine($"The SHA1 hash of {username}{apiKey}{currentTimeStamp} is: {authorizationHash}");

            Console.WriteLine("Making Primary Request...");
            var floorFeedback = await requestHandler.GetRequestResponseAsync<FloorResults>(floorsEndPoint, username, currentTimeStamp, authorizationHash);

            if (floorFeedback.response.Key)
            {
                Console.WriteLine("Primary Request Successfull...");
                Console.WriteLine("Beginning with Fixture Requests...");
                foreach (var item in floorFeedback.result.floors)
                {
                    string currentFloorFixtureEndpoint = fixtureEndPoint.Replace("{floorId}", item.id.ToString());
                    var currentFloorFixtureResults = await requestHandler.GetRequestResponseAsync<FixtureResults>(currentFloorFixtureEndpoint, username, currentTimeStamp, authorizationHash);
                }
            }
            else
            {
                Console.WriteLine($"{floorFeedback.response.Value}");
            }

        }

        private static IConfiguration InitializeConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            return configuration;
        }
    }
}
