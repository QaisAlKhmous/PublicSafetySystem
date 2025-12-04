using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public class CategoryService
    {
        public static IEnumerable<CategoryDTO> GetAllCategories()
        {
            var categories = CategoryRepo.GetAllCategories();

            return categories.Select(c => new CategoryDTO(){
                CategoryId = c.CategoryId,
                Name = c.Name,
            });
        }
    }
}
