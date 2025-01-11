using ApiMongoDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiMongoDB
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions options) : base(options)
        {
        }
        /// <inheritdoc/>

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseMongoDB("mongodb://localhost:27017/pruebas", "Pruebas");
        //    base.OnConfiguring(optionsBuilder);
        //}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Student>()
            //   .HasMany(s => s.CourseList)
            //   .WithOne()
            //   .HasForeignKey("StudentId");

            modelBuilder.Entity<Course>();

            base.OnModelCreating(modelBuilder);

        }

        public DbSet<Course> Courses { get; init; }

        //public DbSet<Student> Students { get; init; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Shipment> Shipments { get; set; }

    }
}
