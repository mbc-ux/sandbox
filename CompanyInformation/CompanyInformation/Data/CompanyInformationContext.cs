#nullable disable
using Microsoft.EntityFrameworkCore;

namespace CompanyInformation.Data
{
    public class CompanyInformationContext : DbContext
    {
        private readonly static string DropRecordsSql;

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

        static CompanyInformationContext()
        {
            DropRecordsSql = @"delete from Employees; delete from Companies;";
        }

        public CompanyInformationContext (DbContextOptions<CompanyInformationContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging()
                          .EnableDetailedErrors()
                          .UseSqlServer(x => x.UseRelationalNulls());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EmployeeHeader>()
                        .HasKey(nameof(EmployeeHeader.EmployeeNumber), 
                                nameof(EmployeeHeader.CompanyId));
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<EmployeeHeader> Employees { get; set; }

        public void ImportCsv(Stream stream)
        {
            Database.ExecuteSqlRaw(DropRecordsSql);
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
                    ImportCsvRecord(record);
                }
                while (currentRecord != null);
            }
            var errorMessage = string.Empty;
            if(IsManagementCompanyChainValid(out errorMessage))
                throw new InvalidOperationException("Invalid management chain discovered: " + errorMessage);
            SaveChanges();
        }

        private bool IsManagementCompanyChainValid(out string errorMessage)
        {
            errorMessage = string.Empty;
            foreach(var companyEmployeeGroup in Employees.Local.GroupBy(m => m.CompanyId))
            {
                var companyId = companyEmployeeGroup.Key;
                foreach(var employee in companyEmployeeGroup)
                {
                    if (!companyEmployeeGroup.Any(m => m.EmployeeNumber == employee.ManagerEmployeeNumber))
                    {
                        errorMessage = $"Could not locate employee:{employee.ManagerEmployeeNumber} in company: {companyId}";
                        return false;
                    }
                }
            }
            return true;
        }

        private void ImportCsvRecord(Record record)
        {
            int companyId;
            if (!int.TryParse(record?.GetValue("CompanyId"), out companyId)) return;
            var company = Companies.Find(companyId);
            if(company == null)
            {
                company = new Company
                {
                    Id = companyId,
                    Code = record?.GetValue("CompanyCode"),
                    Description = record?.GetValue("CompanyDescription")
                };
                Companies.Add(company);
            }
            var employee = Employees.Find(record?.GetValue("EmployeeNumber"), companyId);
            if (employee != null) return; // use the first record for the employee number found in the company.
            var employeeNumber = record?.GetValue("EmployeeNumber");
            employee = new Employee
            {
                EmployeeNumber = employeeNumber,
                FullName = $"{record?.GetValue("EmployeeFirstName")} {record?.GetValue("EmployeeLastName")}",
                Email = record?.GetValue("EmployeeEmail"),
                Department = record?.GetValue("EmployeeDepartment")
            };
            DateTime hireDate;
            if (DateTime.TryParse(record?.GetValue("HireDate"), out hireDate)) employee.HireDate = hireDate;
            var managerEmployeeNumber = record?.GetValue("ManagerEmployeeNumber");
            employee.ManagerEmployeeNumber = string.IsNullOrEmpty(managerEmployeeNumber) ? null : managerEmployeeNumber;
            company.Employees.Add(employee);
            Employees.Add(employee);
        }
    }
}
