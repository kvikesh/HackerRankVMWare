using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeCollection.WebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeCollection.WebAPI.Services
{
    public class EmployeeCollectionService : IEmployeeCollectionService
    {
        private readonly EmployeeCollectionContext _context;

        public EmployeeCollectionService(EmployeeCollectionContext context)
        {
            _context = context;
        }

        public int GetLastEmpployeeId()
        {
            int employees = _context.Employees.Max(x=>x.Id);

            return employees;
        }

        public async Task<List<Employee>> GetEmployees(int[] ids, Filters filters)
        {
            IQueryable<Employee> employees = _context.Employees;

            if (filters == null)
                filters = new Filters();

            if (filters.Location != null && filters.Location.Any())
                employees = employees.Where(x => filters.Location.Contains(x.Location));

            if (filters.Department != null && filters.Department.Any())
                employees = employees.Where(x => filters.Department.Contains(x.Department));

            if (filters.Age != null && filters.Age.Any())
                employees = employees.Where(x => filters.Age.Contains(x.Age));

            if (ids != null && ids.Any())
                employees = employees.Where(x => ids.Contains(x.Id));

            return await employees.OrderBy(o => o.Id).ToListAsync();
        }

        public async Task<Employee> AddEmployee(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteEmployee(Employee employee)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public interface IEmployeeCollectionService
    {
        /// <summary>
        /// Gets all employees
        /// </summary>
        /// <returns>Gets all employees as per the filters</returns>
        Task<List<Employee>> GetEmployees(int[] ids, Filters filters);

        /// <summary>
        /// Add new employee
        /// </summary>
        /// <param name="employee">Employee to add</param>
        /// <returns>Added employee</returns>
        Task<Employee> AddEmployee(Employee employee);

        /// <summary>
        /// Deletes employee by id
        /// </summary>
        /// <param name="employee">employee details</param>
        /// <returns>Status of the delete request</returns>
        Task<bool> DeleteEmployee(Employee employee);

        int GetLastEmpployeeId();
    }

    public class Filters
    {
        public string[] Department { get; set; }
        public string[] Location { get; set; }
        public int[] Age { get; set; }
    }
}