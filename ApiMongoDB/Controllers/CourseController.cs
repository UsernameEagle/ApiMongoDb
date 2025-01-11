using ApiMongoDB.Models;
using ApiMongoDB.Repositories;
using ApiMongoDB.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly ICourseService _CourseService;
        private readonly ApiDbContext _context;

        public CourseController(ICourseService service,
            ApiDbContext context,
            IRepository<Product> productRepository,
            IRepository<Customer> customerRepository,
            IRepository<Shipment> shipmentRepository)
        {
            _CourseService = service;
            _context = context;
            _productRepository = productRepository;
            _shipmentRepository = shipmentRepository;
            _customerRepository = customerRepository;
        }


        /// <summary>
        /// Obtenemos un curso a partir de su identificador
        /// </summary>
        /// <param name="id">Identificador del curso</param>
        /// <returns></returns>
        /// <response code="404">NotFound. No se ha encontrado el curso solicitado</response>
        /// <response code="200">Ok. Devuelve el curso solicitado</response>
        [HttpGet("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id)
        {
            var course = await _CourseService.GetById(id);
            return course is null ? NotFound() : Ok(course);
        }

        /// <summary>
        /// Crea un curso nuevo.
        /// </summary>
        /// <param name="course">JSON con el curso a crear</param>
        /// <returns>Dirección para obtener el curso creado</returns>
        /// <exception cref="Exception">Excepción si hay algún error en la creación del curso</exception>
        /// <response code="201">Created. El curso se crea correctamente</response>        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Course course)
        {
            var createdCourse = await _CourseService.Create(course);

            return createdCourse is null
                ? throw new InvalidOperationException("Course creation failed")
                : CreatedAtAction(nameof(GetById),
                new { id = createdCourse.Id }, createdCourse);
        }


        [HttpPost("AddCustomer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCustomer(Customer customer)
        {
            var created = await _customerRepository.AddAsync(customer);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("GetAllCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCustomer() => Ok(await _customerRepository.GetAllAsync());



        [HttpGet("GetAll")]
        public async Task<ActionResult<Course>> GetAll()
        {
            return Ok(await _CourseService.GetAllCourses());
        }

        [HttpPut("Update")]
        public async Task<ActionResult> Update(Course course)
        {
            return Ok(await _CourseService.UpdateCourse(course));
        }


        [HttpDelete("Delete")]
        public async Task<ActionResult> DeleteById(string id)
        {
            return Ok(await _CourseService.DeleteCourse(id));
        }

        [HttpPost("Transaction")]
        public async Task<ActionResult> Transaction(OperationRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var product = new Product { Name = request.ProductName, Price = request.ProductPrice };
                await _productRepository.AddAsync(product);

                // Modificar cliente
                var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
                if (customer == null)
                {
                    return NotFound("Customer not found");
                }
                customer.Name = request.CustomerName;
                await _customerRepository.UpdateAsync(customer);

                // Crear registro de envío
                var shipment = new Shipment
                {
                    ProductId = product.Id,
                    CustomerId = customer.Id,
                    ShipmentDate = DateTime.UtcNow
                };
                await _shipmentRepository.AddAsync(shipment);

                // Confirmar transacción
                await transaction.CommitAsync();
                return Ok("Operations completed successfully");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, "Error performing operations");
            }

        }

    }
}
