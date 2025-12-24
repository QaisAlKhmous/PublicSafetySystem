using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class YearEmployeeSummaryDTO
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Category { get; set; }
        public Guid CategoryId { get; set; }
        public int TotalEntitled { get; set; }
        public int TotalIssued { get; set; }
        public int TotalRemaining { get; set; }
    }
}
