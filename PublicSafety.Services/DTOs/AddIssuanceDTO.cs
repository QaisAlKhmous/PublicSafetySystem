using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class AddIssuanceDTO
    {
        public Guid IssuanceId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid ItemId { get; set; }
        public Guid? MatrixItemId { get; set; }
        public int Quantity { get; set; }
        public string Type { get; set; }
        public string ExceptionReason { get; set; }
        public string ExceptionFormPath { get; set; }
        public string SignedReceiptPath { get; set; }
        public string IssuanceDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
