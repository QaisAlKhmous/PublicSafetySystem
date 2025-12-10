using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class MatrixItem
    {
        public Guid MatrixItemId { get; set; }
        public Matrix Matrix { get; set; }
        public Guid MatrixId { get; set; }
        public Guid ItemId { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
        public int Frequency { get; set; }
        public User CreatedBy { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate {  get; set; }
        public ICollection<Issuance> Issuances { get; set; }


    }
}
