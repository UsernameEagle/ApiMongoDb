using ApiMongoDB.Models;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace ApiMongoDBTest
{
    public class PersonControllerTestV2 : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public PersonControllerTestV2(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _factory.Server.PreserveExecutionContext = true;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsPersons()
        {
            // Arrange
            var person = new Person { Name = "John Doe", Age = 30 };
            var database = _factory.Services.GetRequiredService<IMongoDatabase>();
            var collection = database.GetCollection<Person>("people");
            await collection.InsertOneAsync(person);

            var kk = await collection.Find(_ => true).ToListAsync();
            // Act
            var response = await _client.GetAsync("/api/person");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var persons = JsonConvert.DeserializeObject<List<Person>>(responseString);
            Assert.Single(persons);
            Assert.Equal("John Doe", persons[0].Name);
        }
    }
}
