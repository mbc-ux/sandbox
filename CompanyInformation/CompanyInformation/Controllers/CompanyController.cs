using CompanyInformation.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            return _context.Companies.Include(nameof(Company.Employees))
                                     .FirstOrDefault(c => c.Id == companyId);
        }

        // GET api/<CompanyController>/5/Employees/1234
        [HttpGet("{companyId}/Employees/{employeeNumber}")]
        public Employee Get(int companyId, string employeeNumber)
        {
            var record = _context.Employees.Find(employeeNumber, companyId);
            if (record == null)
                return null;
            var employee = GetEmployee(record);
            do
            {
                var currentManagerEmployeeNumber = record.ManagerEmployeeNumber;
                record = _context.Employees.Find(currentManagerEmployeeNumber);
                if (record == null) continue;
                employee.Managers.Add(GetEmployee(record));
            } while (record != null);
            return employee;
        }

        private Employee GetEmployee(EmployeeHeader record)
        {
            return new Employee
            {
                CompanyId = record.CompanyId,
                EmployeeNumber = record.EmployeeNumber,
                Email = record.Email,
                Department = record.Department,
                FullName = record.FullName,
                HireDate = record.HireDate
            };
        }
    }
}
