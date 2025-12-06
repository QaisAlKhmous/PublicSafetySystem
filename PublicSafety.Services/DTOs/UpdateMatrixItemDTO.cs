using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class UpdateMatrixItemDTO
    {
        public Guid MatrixItemId { get; set; }
        public int Quantity { get; set; }
        public int Frequency { get; set; }
    }
}
