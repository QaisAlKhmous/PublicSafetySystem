using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class MatrixItemDTO
    {
        public Guid MatrixItemId { get; set; }
        public Guid MatrixId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public int Frequency { get; set; }
        public string CreatedBy { get; set; }
    }
}
