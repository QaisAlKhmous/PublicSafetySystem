using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class EmployeesByCategory
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int EmployeeCount { get; set; }
    }
}
