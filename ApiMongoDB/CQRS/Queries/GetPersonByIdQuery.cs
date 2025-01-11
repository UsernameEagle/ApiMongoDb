using ApiMongoDB.Models;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ApiMongoDB.CQRS.Queries
{
    public record GetPersonByIdQuery(string id) : IRequest<Person>;

    public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, Person>
    {
        private readonly IMongoCollection<Person> _personCollection;

        public GetPersonByIdQueryHandler(IOptions<SchoolDatabaseSettings> schoolDatabaseSettings, IMongoClient client)
        {
            var database = client.GetDatabase(schoolDatabaseSettings.Value.DatabaseName);
            _personCollection = database.GetCollection<Person>("people");
        }

        public async Task<Person> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
        {
            return await (await _personCollection.FindAsync(c => c.Id == request.id)).FirstOrDefaultAsync();
        }
    }

}
