using EnlightedApiConsumer.Data;
using EnlightedApiConsumer.Data.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EnlightedApiConsumer
{
    
    class Program
    {
        private static IConfiguration InitializeConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            return configuration;
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Initializing Configurations...");
            IConfiguration _configuration = InitializeConfig();

            string _baseUrl = _configuration["BaseUrl"];
            string floorsEndPoint = _configuration["FloorsEndPoint"];
            string fixtureEndPoint = _configuration["FixtureEndPoint"];
            string connectionString = _configuration["connectionstring"];
            enlighteddbContext _dbContext = new enlighteddbContext(connectionString);


            IRequestHandler requestHandler = new RequestHandler(_baseUrl);

            /*Test settings for BOB's example*/
            //string date_string = "3/3/2016 7:36:51.032 PM";
            //string username = "bob";
            //string apiKey = "6eb6f07fd09b18dd61dd353dfb669820e7859cd3";

            /*Live settings*/
            string date_string = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
            string username = _configuration["Username"];
            string apiKey = _configuration["ApiKey"];

            long currentTimeStamp = AuthorizationHandler.GetTimeStamp(date_string);
            string authorizationHash = AuthorizationHandler.GetAuthorizationHash(username, apiKey, currentTimeStamp);

            Console.WriteLine($"Authenticating as {username}");
            
            Console.WriteLine("Starting Database Population...");

            await DbWorker.PopulateDatabase(_dbContext, floorsEndPoint, fixtureEndPoint, requestHandler, username, currentTimeStamp, authorizationHash);

        }

       
    }
}
