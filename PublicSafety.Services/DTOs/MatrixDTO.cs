using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class MatrixDTO
    {
        public string Item {  get; set; }
        public string Category { get; set; }

        public int Quantity { get; set; }
        public int Frequency { get; set; }
    }
}
