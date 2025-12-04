using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class JobTitleCategory
    {
        public Guid JobTitleCategoryId { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public Guid JobTitleId { get; set; }
        public JobTitle JobTitle { get; set; }

    }
}
