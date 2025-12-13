using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class Entitlement
    {
        public Guid EmployeeId { get; set; }
        public string FullName { get; set; }
        public Guid MatrixItemId { get; set; }
        public Guid ItemId { get; set; }
        public string CategoryName { get; set; }
        public string ItemName { get; set; }
        public int EntitledQty { get; set; }
        public int EntitlementYear { get; set; }
        public int IssuedQty { get; set; }
        public int RemainingQty { get; set; }
        public int IsIssued { get; set; }
    }
}
