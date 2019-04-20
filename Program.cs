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
using Serilog.Sinks.SumoLogic;
using System.Diagnostics;

namespace myApp
{
    class Program
    {
        static IConfiguration config;
        static Serilog.Core.Logger Log = null;
        static void Main(string[] args)
        {            
            int nsecs = 1000;
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
//Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
//Serilog.Debugging.SelfLog.Enable(Console.Error);

// File logger
/*
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
 */
 /* 
            var Log = new LoggerConfiguration()
            .WriteTo.SumoLogic("https://endpoint4.collection.us2.sumologic.com/receiver/v1/http/ZaVnC4dhaV1OJMaHi0tURR0qnAZ2G4CmmLdC4MdjYUbmtlFDnp5jAd9h4z0AbkVsIYNHswsCLX87SsDb9hZsW_aY6umdGIQOjPoNMeoUflRQlGL4q6LYVQ==")
                //.WriteTo.SumoLogic("http://localhost", textFormatter: new MessageTemplateTextFormatter("FOOBAR", null))
                .WriteTo.MongoDBCapped()
                .CreateLogger();
    */
            // From appsettings.json
      
            Log = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)                
                .CreateLogger();   

        /**
var Log = new LoggerConfiguration()
    //.WriteTo.MongoDB("mongodb://mymongodb/logs")
    .WriteTo.MongoDB("mongodb://localhost:27017/logs")
    //.WriteTo.MongoDBCapped("mongodb://localhost:27017/logs", cappedMaxSizeMb: 50, cappedMaxDocuments: 7)
    .MinimumLevel.Verbose()    
    //.MinimumLevel.Fatal()    
    .CreateLogger();             
**/

                //Console.WriteLine("The current time is " + DateTime.Now);
                showVitals();            

                listStudents();
                ListCollectionAsync().Wait();               
           
                //Console.ReadLine();
                Log.Verbose("VVVVVVVV Verbose");
                
                Console.WriteLine($"'waiting {nsecs} milli secs."); Task.Delay(nsecs).Wait(); // Wait 2 seconds with blocking
                Log.Debug(  "DDDDDDDD Debug");
Console.WriteLine($"'waiting {nsecs} milli secs."); Task.Delay(nsecs).Wait(); // Wait 2 seconds with blocking
                Log.Information(  "IIIIIIII Information");
Console.WriteLine($"'waiting {nsecs} milli secs."); Task.Delay(nsecs).Wait(); // Wait 2 seconds with blocking                
                Log.Warning("WWWWWWWW Warning");
                Log.Fatal(  "FFFFFFFF Fatal terminated unexpectedly");
                Log.Error(  "EEEEEEEE Error");
                Log.Verbose("VVVVVVVV Error");


                Log.Information("Host IIIIIIIIIIII");
                nsecs  =6000;
                Console.WriteLine($"'waiting {nsecs} milli secs. The end is near."); Task.Delay(nsecs).Wait(); // Wait 2 seconds with blocking                

            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n\n catch catch\n{ex}");
                if (Log != null) { Log.Fatal($"catch catch\n {ex}"); }
                //return;
            }  
            finally
            {
                if (Log != null) { Log.Information("Fulsh"); }
                //Log.Information("Fulsh");
                //Log.CloseAndFlush();
            }      

            Console.WriteLine($"'waiting {nsecs} milli secs. The end is near."); Task.Delay(nsecs).Wait(); // Wait 2 seconds with blocking                    
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

            //throw new Exception("FM man made exception!!!!");

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

            //Console.WriteLine("\n2222222222222222222222222222\n\n");

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