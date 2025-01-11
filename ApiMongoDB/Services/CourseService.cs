using ApiMongoDB.Models;
using ApiMongoDB.Repositories;

namespace ApiMongoDB.Services
{
    public class CourseService : ICourseService
    {
        //private readonly IMongoCollection<Course> _courseCollection;
        //public CourseService(IOptions<SchoolDatabaseSettings> schoolDatabaseSettings, IMongoClient client)
        //{
        //    var database = client.GetDatabase(schoolDatabaseSettings.Value.DatabaseName);
        //    _courseCollection = database.GetCollection<Course>(schoolDatabaseSettings.Value.CoursesCollectionName);
        //}

        private readonly ICourseRepository _repository;
        public CourseService(ICourseRepository repository)
        {
            _repository = repository;
        }

        public async Task<Course?> Create(Course course) => await _repository.AddAsync(course);

        public async Task<Course?> GetById(string id) => await _repository.GetByIdAsync(id);

        public async Task<IEnumerable<Course>?> GetAllCourses() => await _repository.GetAllAsync();

        public async Task<bool> DeleteCourse(string id) => await _repository.DeleteAsync(id);

        public async Task<Course> UpdateCourse(Course course) => await _repository.UpdateAsync(course);


    }
}
