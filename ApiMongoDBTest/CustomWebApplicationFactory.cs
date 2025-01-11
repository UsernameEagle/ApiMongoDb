using ApiMongoDB.CQRS.Queries;
using ApiMongoDB.Models;
using ApiMongoDB.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Reflection;

namespace ApiMongoDBTest
{
    public class CustomWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        private const string BEARER_TOKEN =
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwYWFkMjcwNS1lMTE5LTRmZDAtODhmNi04YzU5Zjc4MTA1NmQiLCJ1c2VySWQiOiJjdXN0b21zSW50ZWdyYXRpb24iLCJ1c2VyTG9naW4iOiJwZWRyb3BlIiwibmFtZSI6IlVzdWFyaW8gZGUgaW50ZWdyYWNpw7NuIGFkdWFuYXMiLCJlbWFpbCI6ImN1c3RvbXNAY3VzdG9tcy5lcyIsIm9yZ2FuaXphdGlvbiI6IjIyOCIsIm9yZ2FuaXphdGlvbkNvZGUiOiJYRFMiLCJvcmdhbml6YXRpb25OYW1lIjoiQ3VzdG9tcyIsImlzSW50ZWdyYXRpb25Vc2VyIjoidHJ1ZSIsImFjdGlvbnMiOlsiTFNQOlsxOzJdIiwiTWVzc2FnaW5nOlsxXSJdLCJzY29wZSI6Im9wZW5pZCIsImlzcyI6Imh0dHBzOi8vYnVpbGRwY3Mtc2VjdXJpdHk6OTQ0NC9vYXV0aDIvdG9rZW4iLCJhdWQiOiJQZUFqa2NxWTVHYnJJYm5NZlRCX1NVTHRjU0VhIiwibmJmIjoxNjk4NzU0NzYyLCJhenAiOiJQZUFqa2NxWTVHYnJJYm5NZlRCX1NVTHRjU0VhIiwiZXhwIjoxNjk4NzU4MzYyLCJpYXQiOjE1MTYyMzkwMjIsImp0aSI6IjYxYWExZDc2LTBmODAtNGRlNS1iNTNjLWQ2MGZhYTljNWY3NCJ9.QRghciCYdYGedgDoc-P0KqKX0U39lcPegJsrN4vLnEQ";

        // public HttpClient httpClient { get; private set; } = default!;

        private readonly MongoDbRunner _mongoRunner;
        public readonly string _databaseName = "TestingDb";
        public readonly IMongoDatabase MongoDatabase;
        public readonly IMongoCollection<Course> CourseCollection;
        public readonly IMongoCollection<Person> PersonCollection;

        public readonly IMongoClient _mongoClient;

        public CustomWebApplicationFactory()
        {
            //_mongoRunner = MongoDbRunner.StartForDebugging(singleNodeReplSet: true);
            _mongoRunner = MongoDbRunner.Start(singleNodeReplSet: true);
            _mongoClient = new MongoClient(_mongoRunner.ConnectionString);
            MongoDatabase = _mongoClient.GetDatabase(_databaseName);
            PersonCollection = MongoDatabase.GetCollection<Person>("people");
            CourseCollection = MongoDatabase.GetCollection<Course>("course");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(IMongoClient));
                services.RemoveAll(typeof(IMongoDatabase));
                services.RemoveAll(typeof(IOptions<SchoolDatabaseSettings>));
                services.RemoveAll(typeof(IMongoDatabase));

                services.AddSingleton<IMongoClient>(new MongoClient(_mongoRunner.ConnectionString));
                services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(_databaseName));

                services.Configure<SchoolDatabaseSettings>(options =>
                {
                    options.ConnectionString = _mongoRunner.ConnectionString;
                    options.DatabaseName = _databaseName;
                    options.PersonCollectionName = "people";
                });

                services.AddScoped<IPersonService, PersonService>();
                services.AddSingleton<Instrumentor>();
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllPersonQuery).GetTypeInfo().Assembly));

                ////Intento de configurar el mongo en EF para test
                //// Elimina el contexto de base de datos existente
                //var descriptor = services.SingleOrDefault(
                //    d => d.ServiceType == typeof(DbContextOptions<ApiDbContext>));
                //if (descriptor != null)
                //    services.Remove(descriptor);

                //// Agrega el contexto de base de datos con Mongo2Go
                //services.AddDbContext<ApiDbContext>(options =>
                //    options.UseMongoDB(_mongoRunner.ConnectionString, "TestingDb"));
            });

            ////Intento de configurar el mongo en EF para test
            //builder.ConfigureAppConfiguration((context, config) =>
            //{                
            //    var inMemorySettings = new List<KeyValuePair<string, string?>>();
            //    var inMS1 = new KeyValuePair<string, string?>("SchoolDatabaseSettings:ConnectionString", _mongoRunner.ConnectionString);
            //    var inMS2 = new KeyValuePair<string, string?>("SchoolDatabaseSettings:DatabaseName", "TestingDb");

            //    inMemorySettings.Append(inMS1);
            //    inMemorySettings.Append(inMS2);

            //    config.AddInMemoryCollection(inMemorySettings);
            //});

            base.ConfigureWebHost(builder);

        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);


            return host;
        }

        public void SeedData(string file)
        {
            //var documentCount = PersonCollection.CountDocuments(Builders<Person>.Filter.Empty);
            //if (documentCount == 0)
            //    _mongoRunner.Import(_databaseName, "people", file, true);
            var jsonData = File.ReadAllText(file);
            var people = JsonConvert.DeserializeObject<List<Person>>(jsonData);

            if (people != null && people.Any())
                PersonCollection.InsertMany(people);
        }


        //public async Task InitializeAsync()
        //{
        //    httpClient = CreateClient();
        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BEARER_TOKEN);
        //}


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _mongoClient.DropDatabase(_databaseName);

            _mongoRunner?.Dispose();
        }
    }
}
