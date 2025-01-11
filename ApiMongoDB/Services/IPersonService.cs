using ApiMongoDB.Models;

namespace ApiMongoDB.Services
{
    public interface IPersonService
    {
        Task<Person?> Create(Person person);
        Task<Person?> GetById(string id);
        Task<IEnumerable<Person>?> GetAllPeople();
    }
}
