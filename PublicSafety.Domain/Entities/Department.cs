using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class Department
    {
        public Guid DepartmentId { get; set; }
        public string Name { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public ICollection<SectionJobTitle> SectionJobTitles { get; set; }
        public ICollection<DepartmentJobTitle> DepartmentJobTitles { get; set; }
        public ICollection<Section> Sections { get; set; }
    }
}
