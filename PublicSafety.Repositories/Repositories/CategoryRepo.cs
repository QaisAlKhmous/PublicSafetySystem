using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
