using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class EntitlementPaged
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }

        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public Guid? SectionId { get; set; }
        public string SectionName { get; set; }

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }

        public Guid ItemId { get; set; }
        public string ItemName { get; set; }

        public Guid MatrixItemId { get; set;}

        public int EntitledQty { get; set; }
        public int IssuedQty { get; set; }
        public int RemainingQty { get; set; }

        public int EntitlementYear { get; set; }

        public bool IsIssued { get; set; }
    }
}
