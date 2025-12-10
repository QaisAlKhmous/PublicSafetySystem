using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public enum enEntityType
    {
        Employee = 0,
        Item = 1,
        Matrix = 2,
        Issuance = 3
    }
    public enum enRequestStatus
    {
        pending = 0,
        approved= 1,
        rejected = 2
    }
    public class ChangeRequest
    {
        public Guid RequestId { get; set; }
        public enEntityType EntityType { get; set; }     
        public Guid EntityId { get; set; }         
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public User ChangedBy { get; set; } 
        public Guid ChangedById { get; set; }
        public User ApprovedBy { get; set; }
        public Guid? ApprovedById { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public enRequestStatus Status { get; set; }    
        public string AdminComment { get; set; }
    }
}
