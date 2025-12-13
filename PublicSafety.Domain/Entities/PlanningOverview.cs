using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class PlanningOverview
    {
        public int Year { get; set; }

        public int EmployeesCount { get; set; }  

        public int Issued { get; set; }
        public int Planned { get; set; }
    }
}
