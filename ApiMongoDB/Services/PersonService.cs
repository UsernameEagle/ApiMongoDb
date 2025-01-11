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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schoolDatabaseSettings"></param>
        /// <param name="client"></param>        
        public PersonService(IOptions<SchoolDatabaseSettings> schoolDatabaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(schoolDatabaseSettings.Value.DatabaseName);
            _personCollection = database.GetCollection<Person>("people");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Person?> GetById(string id) => await (await _personCollection.FindAsync(c => c.Id == id)).FirstOrDefaultAsync();

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
                await _personCollection.InsertOneAsync(person);
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
                return await _personCollection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

        }
    }
}
