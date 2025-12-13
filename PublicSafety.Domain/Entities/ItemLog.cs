using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public enum enItemActionType
    {
        Entitled = 1,
        Exception = 2,
        Damaged = 3,
        Disposed = 4,
        StockIncrease = 5
    }
    public class ItemLog
    {
        public Guid ItemLogId { get; set; }

        public Guid? EmployeeId { get; set; }
        public Guid ItemId { get; set; }

        public Guid? MatrixItemId { get; set; }
        public Guid? IssuanceId { get; set; }

        public enItemActionType ActionType { get; set; }
        public int Quantity { get; set; }

        public int? EntitlementYear { get; set; }

        public string Notes { get; set; }

        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties
        public virtual Employee Employee { get; set; }
        public virtual Item Item { get; set; }
        public virtual MatrixItem MatrixItem { get; set; }
        public virtual Issuance Issuance { get; set; }
        public virtual User CreatedBy { get; set; }
    }

}
