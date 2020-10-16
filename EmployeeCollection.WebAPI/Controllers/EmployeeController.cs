using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using EmployeeCollection.WebAPI.Data;
using EmployeeCollection.WebAPI.Services;
using Microsoft.AspNetCore.Http;

namespace EmployeeCollection.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeCollectionService _empService;
        public EmployeeController(IEmployeeCollectionService empService)
        {
            _empService = empService;
        }

        [HttpPost]
        public JsonResult AddEmployee(Employee employee)
        {
            try {
                int lastId = this._empService.GetLastEmpployeeId();
                employee.Id = lastId + 1;
                this._empService.AddEmployee(employee);

                return new JsonResult("Success")
                {
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch
            {
                return new JsonResult("Failue")
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

        }


        [HttpGet()]
        public async Task<JsonResult> GetAllEmployeeWithDeptFilter()
        {
            try
            {

                string department = HttpContext.Request.Query["department"];
                var filters = new Filters();
                filters.Department = department?.Split(',').ToArray();
                var records = await this._empService.GetEmployees(null, filters);

                return new JsonResult(records)
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch
            {
                return new JsonResult("Failue")
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

        }

        [HttpGet("{id}")]
        public async Task<JsonResult> GetEmployeeWithId(int id)
        {
            try
            {
                int[] ids= new int[] {id};
                var records = await this._empService.GetEmployees(ids, null);
                if (records.Count > 0)
                {
                    return new JsonResult(records)
                    {
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                else {
                    return new JsonResult(records)
                    {
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }
            }
            catch
            {
                return new JsonResult("Failue")
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

        }

        [HttpDelete("{id}")]
        public async Task<JsonResult> DeleteEmployeeWithId(int id)
        {
            try
            {
                int[] ids = new int[] { id };
                var records = await this._empService.GetEmployees(ids, null);
                if (records.Count == 0) {
                    return new JsonResult("Record not found!")
                    {
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                var result = await this._empService.DeleteEmployee(records[0]);
                return new JsonResult("Recorded Deleted")
                {
                    StatusCode = StatusCodes.Status204NoContent
                };
            }
            catch
            {
                return new JsonResult("Failue")
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

        }
    }

}
