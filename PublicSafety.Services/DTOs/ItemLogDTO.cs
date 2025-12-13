using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class ItemLogDTO
    {
        public Guid ItemLogId { get; set; }

        public Guid ItemId { get; set; }
        public string ItemName { get; set; }

        public string EmployeeName { get; set; }   // قد يكون null
        public string ActionType { get; set; }

        public int Quantity { get; set; }
        public int? EntitlementYear { get; set; }

        public string Notes { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }
}
