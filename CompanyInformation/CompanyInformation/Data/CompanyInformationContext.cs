#nullable disable
using Microsoft.EntityFrameworkCore;

namespace CompanyInformation.Data
{
    public class CompanyInformationContext : DbContext
    {
        private class Record
        {
            private readonly IDictionary<string, int> fieldNames;
            private readonly string[] fieldValues;

            internal Record(IDictionary<string, int> fieldNames, string[] fieldValues)
            {
                this.fieldNames=fieldNames;
                this.fieldValues=fieldValues;
            }

            internal string GetValue(string fieldName)
            {
                return fieldValues[fieldNames[fieldName]];
            }
        }

        public CompanyInformationContext (DbContextOptions<CompanyInformationContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public void Import(Stream stream)
        {
            stream.Position = 0;
            using(var reader = new StreamReader(stream))
            {
                var currentRecord = string.Empty;
                var isHeaderRecord = true;
                IDictionary<string, int> headerParts = null;
                do
                {
                    currentRecord = reader.ReadLine();
                    if (currentRecord == null) continue;
                    var parts = currentRecord.Split(',');
                    if(isHeaderRecord)
                    {
                        var currentIndex = 0;
                        headerParts = parts.ToDictionary(i => i, i => currentIndex++);
                        isHeaderRecord = false;
                        continue;
                    }
                    var record = new Record(headerParts, parts);
                    ImportRecord(record);
                }
                while (currentRecord != null);
            }
            SaveChanges();
        }

        private void ImportRecord(Record record)
        {
            var company = Companies.Find(record?.GetValue("CompanyId"));
            if(company == null)
            {
                company = new Company
                {
                    Id = record?.GetValue("CompanyId"),
                    Code = record?.GetValue("CompanyCode"),
                    Description = record?.GetValue("CompanyDescription")
                };
                Companies.Add(company);
            }
            var employee = Employees.Find(record?.GetValue("EmployeeNumber"));
            if (employee != null) return; // use the first record for the employee number found in the company.            
            employee = new Employee
            {
                EmployeeNumber = record?.GetValue("EmployeeNumber"),
                FullName = $"{ record?.GetValue("EmployeeFirstName")} { record?.GetValue("EmployeeLastName")}",
                Email = record?.GetValue("EmployeeEmail"),
                Department = record?.GetValue("EmployeeDepartment")
            };
            DateTime hireDate;
            if (DateTime.TryParse(record?.GetValue("HireDate"), out hireDate)) employee.HireDate = hireDate;
            employee.Managers.Add(new Employee { EmployeeNumber = record?.GetValue("ManagerEmployeeNumber") });
            company.Employees.Add(employee);
            Employees.Add(employee);
        }
    }
}
