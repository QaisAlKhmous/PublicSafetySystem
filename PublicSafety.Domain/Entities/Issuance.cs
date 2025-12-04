using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
  
   public enum enIssuanceType
    {
        Entitled = 0,
        Exception = 1,
        Damaged = 2
    }
   
    public class Issuance
    {
        public Guid IssuanceId { get; set; }
        public Employee Employee { get; set; }
        public Guid EmployeeId { get; set; }
        public Item Item { get; set; }
        public Guid ItemId {  get; set; }
        public int Quantity { get; set; }
        public DateTime IssuanceDate { get; set; }
        public string ExceptionFormPath { get; set; }
        public string SignedReceiptPath { get; set; }
        public DateTime CreatedDate { get; set; }

        public User CreatedBy { get; set; }
        public Guid CreatedById {  get; set; }

        public enIssuanceType Type { get; set; }
        public string ExceptionReason { get; set; }

    }
}
