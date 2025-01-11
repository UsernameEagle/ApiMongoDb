using ApiMongoDB.Models;
using ApiMongoDB.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        //private readonly IBackgroundJobClient _backgroundJobClient;
        //private readonly IRecurringJobManager _recurringJobManager;
        public StudentsController(IStudentService studentService,
            ICourseService courseService)
        //, IBackgroundJobClient backgroundJobClient,
        //IRecurringJobManager recurringJobManager)
        {
            _studentService = studentService;
            _courseService = courseService;
            //_backgroundJobClient = backgroundJobClient;
            //_recurringJobManager = recurringJobManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAll()
        {  //=> Ok(await _studentService.GetAll());

            var students = await _studentService.GetAll();
            if (students is null)
            {
                return NotFound();
            }
            foreach (var student in students)
            {
                student.CourseList ??= new();
                foreach (var courseId in student.Courses)
                {
                    var course = await _courseService.GetById(courseId) ?? throw new ArgumentException("Invalid Course Id");
                    student.CourseList.Add(course);
                }
            }
            return Ok(students);
        }


        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Student>> GetById(string id)
        {
            var student = await _studentService.GetById(id);
            if (student is null)
            {
                return NotFound();
            }
            //if (student.CourseList is null || student.CourseList?.Count == 0)
            //{
            //    return Ok(student);
            //}
            student.CourseList ??= new();
            foreach (var courseId in student.Courses)
            {
                var course = await _courseService.GetById(courseId) ?? throw new ArgumentException("Invalid Course Id");
                student.CourseList.Add(course);
            }
            return Ok(student);
        }


        [HttpPost]
        public async Task<IActionResult> Create(Student student)
        {
            var createdStudent = await _studentService.Create(student);
            return createdStudent is null
                ? NotFound()
                : CreatedAtAction(nameof(GetById),
                new { id = createdStudent.Id }, createdStudent);
        }


        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Student updatedStudent)
        {
            var queriedStudent = await _studentService.GetById(id);

            if (queriedStudent is null)
            {
                return NotFound();
            }
            await _studentService.Update(id, updatedStudent);

            return NoContent();
        }


        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var student = await _studentService.GetById(id);
            if (student is null)
            {
                return NotFound();
            }
            await _studentService.Delete(id);
            return NoContent();
        }
    }
}
