using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EmployeeCollection.WebAPI.Data
{
    public class EmployeeCollectionContext : DbContext
    {
        public EmployeeCollectionContext(DbContextOptions<EmployeeCollectionContext> options)
            : base(options)
        { }

        public DbSet<Employee> Employees { get; set; }
    }

    public class Employee
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public float Rating { get; set; }

        public string Department { get; set; }

        public int Age { get; set; }
    }
}
