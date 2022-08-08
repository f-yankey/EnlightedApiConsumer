using EnlightedApiConsumer.Data.Models;
using EnlightedApiConsumer.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnlightedApiConsumer.Data
{
    public static class DbWorker
    {
        public static string GetCurrentDateTimeString()
        {
            string date_string = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
            return date_string;
        }
        public static async Task<KeyValuePair<bool,string>> MakeApiCallsAndPopulateDatabase(enlighteddbContext _dbContext, string floorsEndPoint, string fixtureEndPoint, IRequestHandler requestHandler, string username, string apiKey)
        {
            Console.WriteLine("Making Floors Request...");

            (KeyValuePair<bool, string> response, FloorResults? result) floorFeedback = await requestHandler.GetRequestResponseAsync<FloorResults>(floorsEndPoint, username, GetCurrentDateTimeString(), apiKey);
            
            Console.WriteLine($"{floorFeedback.response.Value}");

            if (floorFeedback.response.Key)
            {
                Console.WriteLine("Primary Request Successfull...");
                
                foreach (Models.Floor floor in floorFeedback.result.floors)
                {
                    Floor floorInDb = GetFloorInDb(_dbContext, floor);
                    if (floorInDb == null)
                    {
                        /*Add floor if it does not exist in database*/
                        try
                        {
                            floorInDb = AddNewFloor(_dbContext, floor);
                            Console.WriteLine($"New Floor Added! FloorID: {floorInDb.FloorId}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed!!! New Floor with FloorID: {floorInDb.FloorId} could not be added. Error Message: {ex.Message} ");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"\nFloor with ID: {floorInDb.FloorId} already exists! Updating...");
                        /*Update floor if it already exists*/
                        try
                        {
                            floorInDb = UpdateFloor(_dbContext, floor);
                            Console.WriteLine($"Floor with ID: {floorInDb.FloorId} successfully updated!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed!!! Update Failed for Floor with ID: {floorInDb.FloorId}. Error Message: {ex.Message} ");
                        }
                    }

                    Console.WriteLine($"Fetching Fixtures for Floor with ID: {floorInDb.FloorId}...");

                    string currentFloorFixtureEndpoint = fixtureEndPoint.Replace("{floorId}", floor.Id.ToString());
                    (KeyValuePair<bool, string> response, FixtureResults? result) fixtureFeedback = await requestHandler.GetRequestResponseAsync<FixtureResults>(currentFloorFixtureEndpoint, username, GetCurrentDateTimeString(), apiKey);

                    if (fixtureFeedback.response.Key)
                    {
                        List<Data.Models.Fixture> fixtures = fixtureFeedback.result.fixtures;

                        Console.WriteLine($"Adding Fixtures for Floor with ID: {floorInDb.FloorId}");

                        foreach (Models.Fixture fix in fixtures)
                        {
                            Fixture fixtureInDb = GetFixtureInDb(_dbContext, floorInDb, fix);

                            if (fixtureInDb == null)
                            {
                                try
                                {
                                    fixtureInDb = AddNewFixture(_dbContext, floorInDb, fix);
                                    Console.WriteLine($"New Fixture Added! FixtureID: {fixtureInDb.FixtureId} FixtureName: {fixtureInDb.Name}");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Failed!!! New Fixture with FixtureID: {fixtureInDb.FixtureId} and FixtureName: {fixtureInDb.Name} could not be added. Error Message: {ex.Message} ");
                                }
                            }
                            else
                            {
                                try
                                {
                                    Console.WriteLine($"Fixture with ID: {fixtureInDb.FixtureId} and Name: {fixtureInDb.Name} already exists! Updating...");
                                    UpdateFixture(_dbContext, floorInDb, fix);
                                    Console.WriteLine($"Fixture with ID: {fixtureInDb.FixtureId} successfully updated!");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Failed!!! Update Failed for Fixture with ID: {fix.Id}. Error Message: {ex.Message} ");
                                }

                            }
                        }
                        
                    }

                }

                return new KeyValuePair<bool, string>(true, $"Database Successfully Updated!");
            }
            else
            {
                /*A Recursion until the population is successful*/
                return await MakeApiCallsAndPopulateDatabase(_dbContext, floorsEndPoint, fixtureEndPoint, requestHandler, username, apiKey);
            }
        }

        

        private static Fixture AddNewFixture(enlighteddbContext _dbContext, Floor floorInDb, Models.Fixture fix)
        {
            Fixture fixtureInDb = new Fixture
            {
                FixtureId = fix.Id,
                FloorId = floorInDb.FloorId,
                Name = fix.Name,
                Xaxis = fix.Xaxis,
                Yaxis = fix.Yaxis,
                GroupId = fix.GroupId,
                MacAddress = fix.MacAddress,
                ClassName = fix.ClassName
            };
            _dbContext.Fixtures.Add(fixtureInDb);
            _dbContext.SaveChanges();
            Console.WriteLine($"Floor with ID: {floorInDb.FloorId} fixtures saved");
            return fixtureInDb;
        }

        private static Fixture UpdateFixture(enlighteddbContext dbContext, Floor floorInDb, Models.Fixture fixture)
        {
            Fixture record = GetFixtureInDb(dbContext, floorInDb, fixture);

            record.Name = fixture.Name;
            record.Xaxis = fixture.Xaxis;
            record.Yaxis = fixture.Yaxis;
            record.GroupId = fixture.GroupId;
            record.MacAddress = fixture.MacAddress;
            record.ClassName = fixture.ClassName;
            
            dbContext.Entry(record).State = EntityState.Modified;
            dbContext.SaveChanges();

            return record;
        }

        private static Fixture GetFixtureInDb(enlighteddbContext _dbContext, Floor floorInDb, Data.Models.Fixture fix)
        {
            return _dbContext.Fixtures.Where(x => x.FixtureId == fix.Id && x.FloorId == floorInDb.FloorId).FirstOrDefault();
        }

        private static Floor AddNewFloor(enlighteddbContext _dbContext, Data.Models.Floor floor)
        {
            Floor floorInDb = new Floor
            {
                FloorId = floor.Id,
                Name = floor.Name,
                Building = floor.Building,
                Campus = floor.Campus,
                Company = floor.Company,
                Description = floor.Description,
                FloorPlanUrl = floor.FloorPlanUrl,
                ParentFloorId = floor.ParentFloorId
            };
            _dbContext.Floors.Add(floorInDb);
            _dbContext.SaveChanges();
            return floorInDb;
        }

        private static Floor UpdateFloor(enlighteddbContext dbContext, Models.Floor floor)
        {
            Floor record = GetFloorInDb(dbContext, floor);

            record.Name = floor.Name;
            record.Building = floor.Building;
            record.Campus = floor.Campus;
            record.Company = floor.Company;
            record.Description = floor.Description;
            record.FloorPlanUrl = floor.FloorPlanUrl;
            record.ParentFloorId = floor.ParentFloorId;

            dbContext.Entry(record).State = EntityState.Modified;
            dbContext.SaveChanges();

            return record;
        }

        private static Floor GetFloorInDb(enlighteddbContext _dbContext, Data.Models.Floor floor)
        {
            return _dbContext.Floors.Where(x => x.FloorId == floor.Id).FirstOrDefault();
        }

    }
}
