using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class AddEmployeeDTO
    {
        public Guid? EmployeeId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsIntern { get; set; }
        public bool Active { get; set; }
        public string Notes { get; set; }
        public string WorkLocation { get; set; }
        public string HealthInsuranceFile { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid SectionId { get; set; }
        public Guid JobTitleId { get; set; }
        public Guid CategoryId { get; set; }
        public string EmploymentDate { get; set; }
        public string JobTitleUpdateDate { get; set; }
    }
}
