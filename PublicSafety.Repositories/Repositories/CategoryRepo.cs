using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace PublicSafety.Repositories.Repositories
{
    public class CategoryRepo
    {

        public static IEnumerable<Category> GetAllCategories()
        {
            using (var context = new AppDbContext())
            {
                return context.Categories.ToList();
            }
        }

        public static JobTitleCategory GetCategoryByJobTitleId(Guid JobTitleId)
        {
            using (var context = new AppDbContext())
            {
                return context.JobTitleCategories.Include(jc => jc.JobTitle).Include(jc => jc.Category).FirstOrDefault(jc => jc.JobTitleId == JobTitleId);
            }
        }

        public static Category GetCategoryById(Guid CategoryId)
        {
            using (var context = new AppDbContext())
            {
                return context.Categories.Find(CategoryId);
            }
        }
    }
}
