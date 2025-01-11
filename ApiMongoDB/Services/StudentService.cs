using ApiMongoDB.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ApiMongoDB.Services
{
    public class StudentService : IStudentService
    {

        /******************************************************
         *      AQUI NO UTILIZAMOS EF CORE COMO EN CURSES     *
         ******************************************************/


        private readonly IMongoCollection<Student> _studentCollection;

        public StudentService(IOptions<SchoolDatabaseSettings> schoolDatabaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(schoolDatabaseSettings.Value.DatabaseName);

            _studentCollection = database.GetCollection<Student>("students");
        }

        public async Task<Student?> Create(Student student)
        {
            //student.Id = ObjectId.GenerateNewId().ToString();
            await _studentCollection.InsertOneAsync(student);
            return student;
        }

        public async Task<DeleteResult> Delete(string id) => await _studentCollection.DeleteOneAsync(s => s.Id == id);

        public async Task<List<Student>> GetAll()
        {
            // =>
            //var filter = Builders<Student>.Filter.Where(x => x.LastName.Contains("prueba")); //("LastName", "prueba");

            //var query = _studentCollection.AsQueryable(); 
            //return query.Where(s => s.LastName == "prueba").Select(x => x.FirstName).ToList();

            return await _studentCollection.Find(x => true).ToListAsync();
            //return await _studentCollection.Find(s => s.LastName.Contains("prueba") || s.LastName == "lasName").ToListAsync(); //true).ToListAsync();        
            //return await _studentCollection.Find(s => true).ToListAsync();        
        }

        public async Task<Student?> GetById(string id) => await _studentCollection.Find(s => s.Id == id).FirstOrDefaultAsync();

        public async Task<Student?> GetById(int id) => await _studentCollection.Find(s => s.Id == id.ToString()).FirstOrDefaultAsync();

        public async Task<ReplaceOneResult> Update(string id, Student student) => await _studentCollection.ReplaceOneAsync(s => s.Id == id, student);

    }
}
