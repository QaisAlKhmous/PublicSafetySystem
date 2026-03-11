using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class AddJobTitleDTO
    {
        public Guid DepartmentId { get; set; }
        public Guid? SectionId { get; set; }
        public Guid CategoryId { get; set; }
        public string JobTitleName { get; set; }
    }
}
