using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class DisposalDTO
    {
        public Guid DisposalId { get; set; }
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public string DisposalDate { get; set; }
        public string DisposalFormPath { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ApprovedBy { get; set; }
    }
}
