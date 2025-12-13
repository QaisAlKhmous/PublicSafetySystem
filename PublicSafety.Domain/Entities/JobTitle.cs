using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class JobTitle
    {
        public Guid JobTitleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } 
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public ICollection<JobTitleCategory> jobTitleCategories { get; set; }
        public ICollection<EmployeeJobTitleHistory> EmployeeJobTitleHistories { get; set; }

    }
}
