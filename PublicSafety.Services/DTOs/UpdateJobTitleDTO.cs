using DocumentFormat.OpenXml.Office2010.PowerPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class UpdateJobTitleDTO
    {
        public Guid EmployeeId { get; set; }
        public Guid JobTitleId { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? SectionId { get; set; }
    }
}
