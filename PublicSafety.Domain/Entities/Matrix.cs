using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class Matrix
    {
        public Guid MatrixId { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public DateTime ValidFrom   { get; set; }
        public DateTime? ValidTo { get; set; }
        public ICollection<MatrixItem> MatrixItems { get; set; }
    }
}
