using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{

    public class IssuedSummary
    {
        public int Total { get; set; }

        public int Entitled { get; set; }
        public int Exception { get; set; }
        public int Damaged { get; set; }
        public int Disposed { get; set; }
    }
    public class PlanningOverview
    {
        public int Year { get; set; }

        public int EmployeesCount { get; set; }
        public int Planned { get; set; }

        public IssuedSummary Issued { get; set; }
    }

}
