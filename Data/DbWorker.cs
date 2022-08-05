using EnlightedApiConsumer.Data.Models;
using EnlightedApiConsumer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnlightedApiConsumer.Data
{
    public static class DbWorker
    {
        public static async Task<KeyValuePair<bool,string>> PopulateDatabase(enlighteddbContext _dbContext, string floorsEndPoint, string fixtureEndPoint, IRequestHandler requestHandler, string username, long currentTimeStamp, string authorizationHash)
        {
            Console.WriteLine("Making Floors Request...");

            var floorFeedback = await requestHandler.GetRequestResponseAsync<FloorResults>(floorsEndPoint, username, currentTimeStamp, authorizationHash);
            
            Console.WriteLine($"{floorFeedback.response.Value}");

            if (floorFeedback.response.Key)
            {
                Console.WriteLine("Primary Request Successfull...");
                
                foreach (var floor in floorFeedback.result.floors)
                {
                    Floor floorInDb = GetFloorInDb(_dbContext, floor);
                    if (floorInDb == null)
                    {
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
                        Console.WriteLine($"Floor with ID: {floorInDb.FloorId} already exists!");
                    }

                    Console.WriteLine($"Fetching Fixtures for Floor with ID: {floorInDb.FloorId}...");

                    string currentFloorFixtureEndpoint = fixtureEndPoint.Replace("{floorId}", floor.id.ToString());
                    var fixtureFeedback = await requestHandler.GetRequestResponseAsync<FixtureResults>(currentFloorFixtureEndpoint, username, currentTimeStamp, authorizationHash);

                    if (fixtureFeedback.response.Key)
                    {
                        List<Data.Models.Fixture> fixtures = fixtureFeedback.result.fixtures;

                        Console.WriteLine($"Adding Fixtures for Floor with ID: {floorInDb.FloorId}");

                        foreach (var fix in fixtures)
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
                                Console.WriteLine($"Fixture with ID: {fixtureInDb.FixtureId} and Name: {fixtureInDb.Name} already exists!");
                            }
                        }
                        
                    }

                }

                return new KeyValuePair<bool, string>(true, $"Database Successfully Updated!");
            }
            else
            {
                /*A Recursion until the population is successful*/
                return await PopulateDatabase(_dbContext, floorsEndPoint, fixtureEndPoint, requestHandler, username, currentTimeStamp, authorizationHash);
            }
        }

        private static Fixture AddNewFixture(enlighteddbContext _dbContext, Floor floorInDb, Data.Models.Fixture fix)
        {
            Fixture fixtureInDb = new Fixture
            {
                FixtureId = fix.id,
                FloorId = floorInDb.FloorId,
                Name = fix.name,
                Xaxis = fix.xaxis,
                Yaxis = fix.yaxis,
                GroupId = fix.groupId,
                MacAddress = fix.macAddress,
                ClassName = fix.ClassName
            };
            _dbContext.Fixtures.Add(fixtureInDb);
            _dbContext.SaveChanges();
            Console.WriteLine($"Floor with ID: {floorInDb.FloorId} fixtures saved");
            return fixtureInDb;
        }

        private static Fixture GetFixtureInDb(enlighteddbContext _dbContext, Floor floorInDb, Data.Models.Fixture fix)
        {
            return _dbContext.Fixtures.Where(x => x.FixtureId == fix.id && x.FloorId == floorInDb.FloorId).FirstOrDefault();
        }

        private static Floor AddNewFloor(enlighteddbContext _dbContext, Data.Models.Floor floor)
        {
            Floor floorInDb = new Floor
            {
                FloorId = floor.id,
                Name = floor.name,
                Building = floor.building,
                Campus = floor.campus,
                Company = floor.company,
                Description = floor.description,
                FloorPlanUrl = floor.floorPlanUrl,
                ParentFloorId = floor.ParentFloorId
            };
            _dbContext.Floors.Add(floorInDb);
            _dbContext.SaveChanges();
            return floorInDb;
        }

        private static Floor GetFloorInDb(enlighteddbContext _dbContext, Data.Models.Floor floor)
        {
            return _dbContext.Floors.Where(x => x.FloorId == floor.id).FirstOrDefault();
        }

    }
}
