using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class DepartmentJobTitle
    {
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }

        public Guid JobTitleId { get; set; }
        public JobTitle JobTitle { get; set; }
    }
}
