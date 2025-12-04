using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class Category
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public ICollection<JobTitleCategory> jobTitleCategories { get; set; }
        public ICollection<Matrix> Matrices { get; set; }
    }
}
