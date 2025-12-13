using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class PlanningItemDetails
    {
        public int Year { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public int PlannedQty { get; set; }
    }
}
