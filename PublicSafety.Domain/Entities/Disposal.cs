using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class Disposal
    {
        public Guid DisposalId {  get; set; }
        public Guid ItemId { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
        public DateTime DisposalDate { get; set; }
        public string DisposalFormPath { get; set; }
        public User CreatedBy { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public User ApprovedBy { get; set; }
        public Guid? ApprovedById { get; set; }
    }
}
