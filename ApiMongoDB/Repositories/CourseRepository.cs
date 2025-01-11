using ApiMongoDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiMongoDB.Repositories
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {

        /**********************************************
         *      UTILIZAMOS ENTITY FRAMEWORK CORE      *
         **********************************************/


        private readonly ApiDbContext _context;
        private readonly DbSet<Course> _dbSet;

        public CourseRepository(ApiDbContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.Courses; // _context.Set<Course>();
        }
        //implementamos la interfaz ICourseRepository con los específico de curso
    }
}
