using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public enum enWorkLocation { Amman = 1, Khaldieh = 2 }
    
    public class Employee
    {
        public Guid EmployeeId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; } 

        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsIntern { get; set; }
        public bool Active { get; set; }
        public string Notes { get; set; }
        public enWorkLocation WorkLocation { get; set; }
        public string HealthInsuranceFile { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid SectionId { get; set; }
        public Department Department { get; set; }
        public Section Section { get; set; }
        public Guid JobTitleId { get; set; }
        public JobTitle JobTitle { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime EmploymentDate { get; set; }
        public DateTime? RetirementDate { get; set; }
        public ICollection<Issuance> Issuances { get; set; }
        public ICollection<EmployeeJobTitleHistory> EmployeeJobTitleHistories { get; set; }
        public DateTime JobTitleUpdateDate { get; set; }
       
    }
}
