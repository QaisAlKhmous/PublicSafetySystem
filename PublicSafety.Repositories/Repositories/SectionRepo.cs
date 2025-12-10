using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Repositories.Repositories
{
    public class SectionRepo
    {

        public static IEnumerable<Section> GetAllSections()
        {
            using (var context = new AppDbContext())
            {
                return context.Sections.ToList();
            }
        }

        public static Section GetSectionByName(string Name)
        {
            using(var context = new AppDbContext())
            {
                return context.Sections.FirstOrDefault(s => s.Name == Name);
            }
        }

        public static Section GetSectionById(Guid Id)
        {
            using (var context = new AppDbContext())
            {
                return context.Sections.Find(Id);
            }
        }
    }
}
