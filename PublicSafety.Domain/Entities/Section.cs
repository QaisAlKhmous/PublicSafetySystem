using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class Section
    {
        public Guid SectionId { get; set; }
        public string Name { get; set; }
        public Department Department { get; set; }
        public Guid DepartmentId { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<SectionJobTitle> SectionJobTitles { get; set; }
    }
}
