using ApiMongoDB.Models;
using MongoDB.Driver;

namespace ApiMongoDB.Services
{
    public interface IStudentService
    {
        Task<Student?> Create(Student student);
        Task<DeleteResult> Delete(string id);
        Task<List<Student>> GetAll();
        Task<Student?> GetById(string id);
        Task<Student?> GetById(int id);
        Task<ReplaceOneResult> Update(string id, Student student);
    }
}
