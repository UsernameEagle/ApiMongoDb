using ApiMongoDB.Models;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;
using System.Net.Http.Json;

namespace ApiMongoDBTest
{
    public class PersonControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public PersonControllerTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            //_client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", CustomWebApplicationFactory<Program>.BEARER_TOKEN);
            _factory.SeedData("./TestData/people.json");
        }


        [Fact]
        public async Task Test_GetPerson_ReturnsPerson()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var person = new Person { Id = id, Name = "John Doe", Age = 44 };

            // Inserta la persona en la base de datos de prueba
            var personCollection = _factory.PersonCollection; // .MongoDatabase.GetCollection<Person>("people");
            await personCollection.InsertOneAsync(person);

            // Act
            var response = await _client.GetAsync($"api/person/{id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var returnedPerson = await response.Content.ReadFromJsonAsync<Person>();
            Assert.NotNull(returnedPerson);
            Assert.Equal("John Doe", returnedPerson.Name);
            Assert.Equal(44, returnedPerson.Age);
        }

        [Fact]
        public async Task AddPerson_ReturnsCreatedResponse()
        {
            // Arrange
            var newPerson = new Person
            {
                Name = "John Doe",
                Age = 30
            };
            var person1 = new Person { Id = ObjectId.GenerateNewId().ToString(), Name = "Alice", Age = 28 };
            var person2 = new Person { Id = ObjectId.GenerateNewId().ToString(), Name = "Bob", Age = 35 };

            var personCollection = _factory.PersonCollection;

            var todos = await personCollection.Find(_ => true).ToListAsync();

            await personCollection.InsertOneAsync(person1);
            await personCollection.InsertOneAsync(person2);

            todos = await personCollection.Find(_ => true).ToListAsync();

            // Act
            var response = await _client.PostAsJsonAsync("/api/person/CreatePerson", newPerson);

            todos = await personCollection.Find(_ => true).ToListAsync();

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetPersonById_ReturnsPerson()
        {
            // Arrange
            var newPerson = new Person
            {
                Name = "Jane Doe",
                Age = 25
            };

            var postResponse = await _client.PostAsJsonAsync("/api/person/CreatePerson", newPerson);
            postResponse.EnsureSuccessStatusCode();
            var createdPerson = await postResponse.Content.ReadFromJsonAsync<Person>();

            // Act
            var getResponse = await _client.GetAsync($"/api/person/{createdPerson?.Id}");

            // Assert
            getResponse.EnsureSuccessStatusCode(); // Status Code 200-299
            var person = await getResponse.Content.ReadFromJsonAsync<Person>();
            person.Should().NotBeNull();
            person.Should().BeEquivalentTo(createdPerson);
            person?.Name.Should().Be("Jane Doe");
            person?.Age.Should().Be(25);
        }

        [Fact]
        public async Task GetAllPersons_ReturnsAllPersons()
        {
            //// Arrange
            var person1 = new Person { Id = ObjectId.GenerateNewId().ToString(), Name = "Alice", Age = 28 };
            var person2 = new Person { Id = ObjectId.GenerateNewId().ToString(), Name = "Bob", Age = 35 };



            var personCollection = _factory.PersonCollection;

            var todos = await personCollection.Find(_ => true).ToListAsync();

            await personCollection.InsertOneAsync(person1);
            await personCollection.InsertOneAsync(person2);

            todos = await personCollection.Find(_ => true).ToListAsync();

            // Act
            var response = await _client.GetAsync("/api/person");

            // Assert
            response.EnsureSuccessStatusCode();
            var persons = await response.Content.ReadFromJsonAsync<List<Person>>();
            Assert.NotNull(persons);
            Assert.True(persons.Count >= 2);
            Assert.Contains(persons, p => p.Name == "Alice" && p.Age == 28);
            Assert.Contains(persons, p => p.Name == "Bob" && p.Age == 35);
        }


        [Fact]
        public async Task AddPerson_transacction()
        {
            //var session = await _factory._mongoClient.StartSessionAsync();
            //session.StartTransaction();
            try
            {
                // Arrange
                var newPerson = new Person
                {
                    Name = "John Doe",
                    Age = 30
                };
                var person1 = new Person { Id = ObjectId.GenerateNewId().ToString(), Name = "Alice", Age = 28 };
                var person2 = new Person { Id = ObjectId.GenerateNewId().ToString(), Name = "Bob", Age = 35 };

                var personCollection = _factory.PersonCollection;

                var todos = await personCollection.Find(_ => true).ToListAsync();





                await personCollection.InsertOneAsync(person1);
                await personCollection.InsertOneAsync(person2);

                todos = await personCollection.Find(_ => true).ToListAsync();

                // Act
                var response = await _client.PostAsJsonAsync("/api/person/CreatePerson", newPerson);

                //await session.CommitTransactionAsync();

                todos = await personCollection.Find(_ => true).ToListAsync();

                // Assert
                response.EnsureSuccessStatusCode(); // Status Code 200-299
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }
            catch (Exception)
            {
                //session.AbortTransaction();
                throw;
            }
        }


    }
}
