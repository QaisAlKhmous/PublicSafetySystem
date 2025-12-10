using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class JobTitleCategoryDTO
    {
        public Guid JobTitleCategoryId { get; set; }
        public string Category { get; set; }
        public string JobTitle { get; set; }
    }
}
