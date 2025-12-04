using PublicSafety.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace PublicSafety.Repositories.Repositories
{
    public class DepartmentRepo
    {
        public static IEnumerable<Department> GetAllDepartments()
        {
            using (var context = new AppDbContext())
            {
                return context.Departments.ToList();
            }
        }
        public static Department GetDepartmentByName(string Name)
        {
            using (var context = new AppDbContext())
            {
                return context.Departments.FirstOrDefault(s => s.Name == Name);
            }
        }
    }
}
