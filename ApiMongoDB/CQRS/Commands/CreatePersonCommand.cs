using ApiMongoDB.Models;
using ApiMongoDB.Services;
using MediatR;

namespace ApiMongoDB.CQRS.Commands
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="person"></param>
    public record CreatePersonCommand(Person person) : IRequest<Person>;

    /// <summary>
    /// 
    /// </summary>
    public class CreatePersoncommandHandler : IRequestHandler<CreatePersonCommand, Person>
    {
        private readonly IPersonService _personService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="personService"></param>
        public CreatePersoncommandHandler(IPersonService personService)
        {
            _personService = personService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Person?> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var person = new Person { Name = request.person.Name, Age = request.person.Age };
            return await _personService.Create(person);
        }

    }
}
