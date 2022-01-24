using CompanyInformation.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CompanyInformation
{
    [ApiController]
    [Authorize]
    [Route("api/Companies")]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyInformationContext _context;

        public CompanyController(CompanyInformationContext context)
        {
            _context = context;
        }

        // GET: api/<CompanyController>
        [HttpGet]
        public IEnumerable<Company> Get()
        {
            return _context.Companies;
        }

        // GET api/<CompanyController>/5
        [HttpGet("{companyId}")]
        public Company Get(int companyId)
        {
            return _context.Companies.Find(companyId);
        }

        // GET api/<CompanyController>/5
        [HttpGet("{companyId}/Employees/{employeeNumber}")]
        public Employee Get(int companyId, string employeeNumber)
        {
            foreach(Employee employee in _context.Companies.Find(companyId)?.Employees)
            {
                if (employee.EmployeeNumber == employeeNumber) return employee;
            }
            return null;
        }
    }
}
