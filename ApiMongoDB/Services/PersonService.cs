using ApiMongoDB.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ApiMongoDB.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class PersonService : IPersonService
    {
        private readonly IMongoCollection<Person> _personCollection;
        private readonly MongoDbContext _mongoDbContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schoolDatabaseSettings"></param>
        /// <param name="client"></param>        
        public PersonService(IOptions<SchoolDatabaseSettings> schoolDatabaseSettings,
            IMongoClient client,
            MongoDbContext mongoDbContext)
        {
            var database = client.GetDatabase(schoolDatabaseSettings.Value.DatabaseName);
            _personCollection = database.GetCollection<Person>("people");
            _mongoDbContext = mongoDbContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Person?> GetById(string id) => await (await _personCollection.FindAsync(_mongoDbContext.Session, c => c.Id == id)).FirstOrDefaultAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Person?> Create(Person person)
        {
            try
            {
                //person.Id = ObjectId.GenerateNewId().ToString();
                await _personCollection.InsertOneAsync(_mongoDbContext.Session, person);
                return person;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task<IEnumerable<Person>?> GetAllPeople()
        {
            try
            {
                return await _personCollection.Find(_mongoDbContext.Session, _ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

        }
    }
}
