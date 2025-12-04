using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<Disposal> Disposals { get; set; }
        public ICollection<Disposal> ApprovedDisposals { get; set; }
        public ICollection<MatrixItem> MatrixItems { get; set; }
        public ICollection<Issuance> Issues { get; set; }
        public ICollection<Item> Items { get; set; }

    }
}
