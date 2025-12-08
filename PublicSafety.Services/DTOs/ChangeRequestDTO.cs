using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class ChangeRequestDTO
    {
        public Guid RequestId { get; set; }
        public string EntityType { get; set; }
        public Guid EntityId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ChangedBy { get; set; }
        public string RequestDate { get; set; }
        public string Status { get; set; }
        public string AdminComment { get; set; }
    }
}
