using EnlightedApiConsumer.Data;
using EnlightedApiConsumer.Data.Models;
using EnlightedApiConsumer.Utils;
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

            /*variable declarations*/
            string _baseUrl = _configuration["BaseUrl"];
            string floorsEndPoint = _configuration["FloorsEndPoint"];
            string fixtureEndPoint = _configuration["FixtureEndPoint"];
            string connectionString = _configuration["connectionstring"];
            IRequestHandler requestHandler = new RequestHandler(_baseUrl);
            
            /*settings for authentication*/
            string username = _configuration["Username"];
            string apiKey = _configuration["ApiKey"];
           
            /*This block discards the database context after use*/
            using (enlighteddbContext _dbContext = new enlighteddbContext(connectionString))
            {
                if (_dbContext.Database.CanConnect())
                {
                    Console.WriteLine("Successful Database Connection...");

                    Console.WriteLine($"Authenticating as {username}");

                    Console.WriteLine("Starting API calls and Database Population...");

                    KeyValuePair<bool, string> databasePopulationResult = await DbWorker.MakeApiCallsAndPopulateDatabase(_dbContext, floorsEndPoint, fixtureEndPoint, requestHandler, username, apiKey);

                    Console.WriteLine($"{databasePopulationResult.Value}");
                }
                else
                {
                    Console.WriteLine("Failed Database Connection. Kindly be sure that your connection string or it's \nparameters(username,password,etc) are correct!");
                }

            }

        }

       
    }
}
