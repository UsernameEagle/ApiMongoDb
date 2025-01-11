using ApiMongoDB.Models;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ApiMongoDB.CQRS.Queries
{
    public record GetAllPersonQuery() : IRequest<List<Person>>;

    public class GetAllPersonQueryHandler : IRequestHandler<GetAllPersonQuery, List<Person>>
    {
        private readonly IMongoCollection<Person> _personCollection;

        public GetAllPersonQueryHandler(IOptions<SchoolDatabaseSettings> schoolDatabaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(schoolDatabaseSettings.Value.DatabaseName);
            _personCollection = database.GetCollection<Person>("people");
        }

        public async Task<List<Person>> Handle(GetAllPersonQuery request, CancellationToken cancellationToken)
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
