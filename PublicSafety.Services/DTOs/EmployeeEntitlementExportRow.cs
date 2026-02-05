using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class EmployeeEntitlementExportRow
    {
        public string EmployeeNumber { get; set; }  
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Category { get; set; }

        public string ItemName { get; set; }

        public int EntitledQty { get; set; }
        public int IssuedQty { get; set; }
        public int RemainingQty { get; set; }
    }
}
