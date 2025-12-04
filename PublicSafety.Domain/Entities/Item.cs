using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class Item
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public User AddedBy { get; set; }
        public Guid AddedById { get; set; }
        public ICollection<MatrixItem> MatrixItems { get; set; }   
        public ICollection<Issuance> Issuances { get; set; }
    }
}
