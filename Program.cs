using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;
using myApp.Models;
using Serilog;
using Serilog.Events;

namespace myApp
{
    class Program
    {
        static IConfiguration config;
        //static IMongoClient _client;  
        //static IMongoDatabase _database;
        static void Main(string[] args)
        {
            try 
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                config = configuration;

                //var configuration = builder.Build();
                // config = new ConfigurationBuilder()
                //     .AddJsonFile("appsettings.json", true, true)
                //     .Build();

            var Log = new LoggerConfiguration()
                //.MinimumLevel.Information()
                //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                //.WriteTo.Console()
                //.WriteTo.RollingFile("log-{Date}.txt") //, fileSizeLimitBytes: 123)    
                .ReadFrom.Configuration(configuration)  
                //.ReadFrom.Configuration(config)     
                //.Enrich.FromLogContext()     
                .CreateLogger();

                Log.Information("Hello, world!");
                Console.WriteLine("The current time is " + DateTime.Now);
                showVitals();            

                //listStudents();
                ListCollectionAsync().Wait();               
           
                //Console.ReadLine();
                Log.Fatal("Host FFFFFFFFFFFFFFF terminated unexpectedly");
                Log.Information("Host IIIIIIIIIIII");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Log.Fatal(ex, "Host terminated unexpectedly");
                return;
            }  
            finally
            {
                Log.Information("Fulsh");
                Log.CloseAndFlush();
            }          
        }

        static void showList(List<Models.MyStudent> list)
        {
            foreach (Models.MyStudent it in list)
            {
                Console.WriteLine(it.name);
            }

            // find
               // Find items where name contains "seat".
        Console.WriteLine("\nFind: xxx where name contains \"Ram\": {0}", 
            list.Find(x => x.name.Contains("Ram")));

        Console.WriteLine("\nExists: Part with Id=44: {0}", 
            list.Exists(x => x.id == 44));
        }

        static async Task ListCollectionAsync()
        {
            //mongodb://<dbuser>:<dbpassword>@ds057254.mlab.com:57254/rdicode
            //mongodb://mastronardif:fm123456>@ds057254.mlab.com:57254/rdicode
            //mongodb://localhost:27017/platerate
            // "mongodb://ds057254.mongolab.com:57254/admin  -u admin -p admin";

            var connectionString = config.GetConnectionString("MyDb");
            //"mongodb://localhost:27017/";

            var client = new MongoClient(connectionString);

            IMongoDatabase db = client.GetDatabase("rdicode");
            var myColl = config["MyCollection"];
            var collection = db.GetCollection<BsonDocument>(myColl);
            //Console.WriteLine($"COUNT= {collection.Count("")}");

            using (IAsyncCursor<BsonDocument> cursor = await collection.FindAsync(new BsonDocument()))
            {
                while (await cursor.MoveNextAsync())
                {
                    IEnumerable<BsonDocument> batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        Console.WriteLine(document);
                        Console.WriteLine();
                    }
                }  
            }

            Console.WriteLine("\n_______________________\n\n");
            //
            //var filter = new BsonDocument("user.givenName: Frank");
            var filter = Builders<BsonDocument>.Filter.Eq("user.username", "fxm");
            var result = collection.Find(filter).ToList();
           //await collection.Find(filter).ForEachAsync(document => Console.WriteLine(document));

           var filter2 = new BsonDocument("cardId", "bk101");

await collection.Find(filter2)
         .ForEachAsync(document => Console.WriteLine(document));

 Console.WriteLine("\n2222222222222222222222222222\n\n");

            /*
var collection2 = db.GetCollection<BsonDocument>("users");
var query = Query<BsonDocument>.EQ(u => u.Address.State, "OR");
MongoCursor<BsonDocument> cursor = collection.Find(query);
 */
            //
        }

        static void listStudents()
        {
            List<Models.MyStudent> list = new List<Models.MyStudent>() {
                    new Models.MyStudent() { id = 1, name = "John" },
                    new Models.MyStudent() { id = 2, name = "Ellen" },
                    new Models.MyStudent() { id = 3, name = "John" },
                    new Models.MyStudent() { id = 44, name = "John" },
                    new Models.MyStudent() { id = 52, name = "Steve" },
                    new Models.MyStudent() { id = 63, name = "Bill" },
                    new Models.MyStudent() { id = 73, name = "Bill" },
                    new Models.MyStudent() { id = 84, name = "Ram" },
                    new Models.MyStudent() { id = 95, name = "Ron" }
                };
            showList(list);
        }
        static void showVitals()
        {
            Console.WriteLine($" Hello { config["name"] } !");
            //Console.WriteLine($" ConnectionStrings= { config["ConnectionStrings:MyDb"] } !");

            Console.WriteLine($" MyDb conn string=  {config.GetConnectionString("MyDb") } !");
             Console.WriteLine($"Collection: { config["MyCollection"] }");
           
        }
    }
}