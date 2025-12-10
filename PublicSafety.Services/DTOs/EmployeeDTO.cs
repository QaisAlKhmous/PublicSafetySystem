using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class EmployeeDTO
    {
        public Guid? EmployeeId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsIntern { get; set; }
        public bool Active { get; set; }
        public string Notes { get; set; }
        public string WorkLocation { get; set; }
        public string HealthInsuranceFile { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string JobTitle { get; set; }
        public string Category { get; set; }
        public string CreationDate { get; set; }
        public string EmploymentDate { get; set; }
        public string RetirementDate { get; set; }
        public string JobTitleUpdateDate { get; set; }
    }
}
