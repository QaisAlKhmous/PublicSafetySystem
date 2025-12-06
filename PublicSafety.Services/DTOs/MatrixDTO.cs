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
        public Guid MatrixId {  get; set; }
        public Guid CategoryId { get; set; }

        public int Version { get; set; }
        public bool IsActive { get; set; }
    }
}
