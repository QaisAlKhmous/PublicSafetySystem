using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class ActivateEmployeeDTO
    {
        public Guid EmployeeId { get; set; }
        public Guid JobTitleId { get; set; }
        public Guid DepartmentId {  get; set; }
        public Guid SectionId { get; set; }
        public string ActivationDate { get; set; }
    }
}
