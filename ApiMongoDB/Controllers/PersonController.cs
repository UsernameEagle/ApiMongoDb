using ApiMongoDB.CQRS.Commands;
using ApiMongoDB.CQRS.Queries;
using ApiMongoDB.Models;
using ApiMongoDB.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text.Json;

namespace ApiMongoDB.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PersonController> _logger;
        private readonly Instrumentor _instrumentor;
        private readonly IPersonService _personService;
        private readonly IMongoClient _client;
        private readonly MongoDbContext _mongoDbContext;

        // private readonly ActivitySource activitySource;
        private static readonly ActivitySource _activitySource = new("Tracing.ApiMongoDB");
        private static List<Person>? _people;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logger"></param>
        /// <param name="instrumentation"></param>        
        public PersonController(IMediator mediator, ILogger<PersonController> logger, Instrumentor instrumentor, IPersonService personService, IMongoClient client, MongoDbContext mongoDbContext)
        {
            _logger = logger;
            _instrumentor = instrumentor;
            //   activitySource = instrumentation.ActivitySource;
            _mediator = mediator;
            _personService = personService;
            _client = client;
            _mongoDbContext = mongoDbContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        [HttpPost("CreatePerson")]
        [ProducesResponseType(typeof(Person), 200)]
        public async Task<IActionResult> CreatePerson([FromBody] Person person)
        {
            //using (var session = await _client.StartSessionAsync())
            //{
            //    session.StartTransaction();

            await _mongoDbContext.StartMongoSessionAsync();

            try
            {

                _instrumentor.LogMessageInformation(_logger, "Creating a new person.");
                var command = new CreatePersonCommand(person);
                var data = await _mediator.Send(command);
                _instrumentor.LogMessageInformation(_logger, $"Person created with ID: {data.Id}");
                //await session.CommitTransactionAsync();

                await _mongoDbContext.CommitTransactionAsync();

                return CreatedAtAction(nameof(Get), new { id = data.Id }, data);
            }
            catch (Exception)
            {
                await _mongoDbContext.AbortTransactionAsync();

                throw;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("CreatePersonFromFile")]
        [ProducesResponseType(typeof(Person), 200)]
        public async Task<IActionResult> CreatePersonFromFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _instrumentor.LogMessageInformation(_logger, "No file uploaded to create person.");
                return BadRequest("No file uploaded.");
            }

            using var stream = new StreamReader(file.OpenReadStream());
            var content = await stream.ReadToEndAsync();
            try
            {
                var person = JsonSerializer.Deserialize<Person>(content, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, PropertyNameCaseInsensitive = true });
                if (person == null)
                {
                    _instrumentor.LogMessageInformation(_logger, "Invalid JSON content to create person from file.");
                    return BadRequest("Invalid JSON content.");
                }

                var command = new CreatePersonCommand(person);
                await _mediator.Send(command);
                return CreatedAtAction(nameof(Get), new { id = person.Id }, person);
            }
            catch (Exception ex)
            {
                _instrumentor.LogMessageError(_logger, $"Error creating person from file. {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Person), 200)]
        public async Task<IActionResult> Get(string id)
        {
            _instrumentor.PersonReadersCounter.Add(1, new KeyValuePair<string, object?>("PersonController", "Get"));
            _instrumentor.LogMessageInformation(_logger, $"Getting person with ID: {id}");
            var query = new GetPersonByIdQuery(id);
            var person = await _mediator.Send(query);
            if (person == null)
            {
                _instrumentor.LogMessageInformation(_logger, $"Person with ID: {id} not found.");
                return NotFound();
            }
            return Ok(person);
        }




        [HttpGet]
        [ProducesResponseType(typeof(Person), 200)]
        public async Task<IActionResult> GetAll()
        {
            _instrumentor.PersonReadersCounter.Add(1, new KeyValuePair<string, object?>("PersonController", "GetAll"));
            using var activity = _activitySource.StartActivity("GetAll");
            activity?.AddTag("getting people", 0);
            _instrumentor.LogMessageInformation(_logger, "Getting all people from logger instrumentor.");
            try
            {
                if (CheckCache(out var people))
                {
                    activity?.AddTag("TotalPeople CheckCache", people.Count());
                    return Ok(people);
                }

                var query = new GetAllPersonQuery();
                _people = (await _mediator.Send(query)).ToList();

                if (_people == null)
                {
                    _instrumentor.LogMessageError(_logger, "No hay registros y eso no puede ser......!");
                    return NotFound();
                }
                //añadir a activitySource una etiqueta con el total de registros
                activity?.AddTag("TotalPeople", _people.Count.ToString());

                return Ok(_people);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                activity?.SetStatus(ActivityStatusCode.Error, "Se ha producido un error");
                activity?.AddException(ex);
                throw;
            }
        }


        private static bool CheckCache(out IEnumerable<Person> people)
        {
            using var activity = _activitySource.StartActivity("CheckCache");
            if (_people is not null)
            {
                activity?.AddTag("people.count", _people.Count);
                people = _people;
                return true;
            }
            people = new List<Person>();
            activity?.AddTag("people en Checkcache", people.Count().ToString());
            return false;
        }


    }
}
