using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class IssueCategoryDTO
    {
        public Guid CategoryId { get; set; }
        public int Year { get; set; }
        public Guid UserId { get; set; }
        public string SignedReceiptPath { get; set; }
    }
}
