using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyInformation
{
    public class CompanyHeader
    {
        /// <summary>
        /// CompanyId
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// CompanyCode
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// CompanyDescription
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Number of Employees in the given company
        /// </summary>
        public int EmployeeCount { get; set; }
    }

    public class EmployeeHeader
    {
        /// <summary>
        /// EmployeeNumber
        /// </summary>
        public string EmployeeNumber { get; set; }
        /// <summary>
        /// "{EmployeeFirstName} {EmployeeLastName}"
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// EmployeeEmail
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Department
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// HireDate
        /// </summary>
        public DateTime? HireDate { get; set; }
        /// <summary>
        /// The EmployeeNumber of the manager
        /// </summary>
        [ForeignKey(nameof(ManagerEmployeeNumber))]
        public string? ManagerEmployeeNumber { get; set; }
        /// <summary>
        /// The Company that the employee is associated with.
        /// </summary>
        public int CompanyId { get; set; }
    }

    public class Company : CompanyHeader
    {
        public Company()
        {
            Employees = new List<EmployeeHeader>();
        }
        /// <summary>
        /// List of EmployeeHeader objects in the given company
        /// </summary>
        public List<EmployeeHeader> Employees { get; }
    }

    public class Employee : EmployeeHeader
    {
        public Employee()
        {
            Managers = new List<EmployeeHeader>();
        }

        /// <summary>
        /// List of EmployeeHeaders of the managers ordered ascending by seniority (i.e. the immediate manager first)
        /// </summary>
        public List<EmployeeHeader> Managers { get; }
    }
}
