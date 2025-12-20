using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class IssueEmployeeYearDTO
    {
        public Guid EmployeeId { get; set; }
        public int Year { get; set; }
        public string SignedReceiptPath { get; set; }
        public Guid CreatedById { get; set; }
    }
}
