using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class EmployeePagedDTO
    {
        public Guid EmployeeId { get; set; }

        public string FullName { get; set; }

        public string EmployeeNumber { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsIntern { get; set; }
        public bool Active { get; set; }
        public string Notes { get; set; }
        public enWorkLocation WorkLocation { get; set; }
        public string WorkLocationStr { get; set; }
        public string HealthInsuranceFile { get; set; }
        public string Department { get; set; }
        public Guid DepartmentId { get; set; }
        public string Section { get; set; }
        public Guid? SectionId { get; set; }
        public string JobTitle { get; set; }
        public Guid JobTitleId { get; set; }
        public string Category { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime CreationDate { get; set; }
        public string EmploymentDateStr { get; set; }
        public DateTime EmploymentDate { get; set; }
        public DateTime RetirementDate { get; set; }
    }
}
