using ApiMongoDB.Models;
using MongoDB.Bson;

namespace ApiMongoDBTest
{
    public class CourseControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CourseControllerTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _factory.SeedData("./TestData/courses.json");
        }

        [Fact]
        public async Task GetCoursesByProductId_Returns200()
        {
            // Arrange            
            // Arrange
            //var id = ObjectId.GenerateNewId().ToString();
            //var person = new Course { Id = id, Name = "John Doe", Code = "XXX" };
            var person = new Course { Name = "John Doe", Code = "XXX" };

            // Inserta la persona en la base de datos de prueba
            var courseCollection = _factory.CourseCollection;
            await courseCollection.InsertOneAsync(person);
            var id = person.Id;
            // Act
            var response = await _client.GetAsync($"api/course/{id}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(200, ((int)response.StatusCode));

            //var returnedPerson = await response.Content.ReadFromJsonAsync<Person>();
            //Assert.NotNull(returnedPerson);
            //Assert.Equal("John Doe", returnedPerson.Name);
            //Assert.Equal(44, returnedPerson.Age);



        }
    }
}
