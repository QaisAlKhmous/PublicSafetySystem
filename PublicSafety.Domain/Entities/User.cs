using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public enum enType
    {
        admin = 0,
        manager = 1,
        user = 2
    }
    public class User
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public enType Type { get; set; }
        public ICollection<Disposal> Disposals { get; set; }
        public ICollection<Disposal> ApprovedDisposals { get; set; }
        public ICollection<MatrixItem> MatrixItems { get; set; }
        public ICollection<Issuance> Issues { get; set; }
        public ICollection<Item> Items { get; set; }
        public ICollection<ChangeRequest> ChangeRequests { get; set; }
        public ICollection<ChangeRequest> ChangeRequestsApproved { get; set; }

    }
}
