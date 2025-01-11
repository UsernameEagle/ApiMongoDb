using ApiMongoDB.Models;

namespace ApiMongoDB.Services
{
    public interface ICourseService
    {
        Task<Course?> Create(Course course);
        Task<Course?> GetById(string id);
        Task<IEnumerable<Course>?> GetAllCourses();
        Task<Course> UpdateCourse(Course course);
        Task<bool> DeleteCourse(string id);
    }
}
