using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Repositories.Repositories
{
    public class JobTitleRepo
    {

        public static IEnumerable<JobTitle> GetAllJobTitles()
        {
            using (var context = new AppDbContext())
            {
                return context.JobTitles.ToList();
            }
        }
        public static JobTitle GetJobTitleByName(string name)
        {
            using (var context = new AppDbContext())
            {
                return context.JobTitles.FirstOrDefault(j => j.Name == name);
            }
        }
        public static JobTitle GetJobTitleById(Guid Id)
        {
            using (var context = new AppDbContext())
            {
                return context.JobTitles.Find(Id);
            }
        }

        public static List<JobTitle> GetJobTitlesByDepartment(Guid departmentId)
        {
            using (var context = new AppDbContext())
            {
                return context.DepartmentJobTitles
              .Where(dj => dj.DepartmentId == departmentId)
              .Select(dj => dj.JobTitle)
              .OrderBy(j => j.Name)
              .ToList();
            }
          
        }

        public static List<JobTitle> GetJobTitlesBySection(Guid SectionId)
        {
            using (var context = new AppDbContext())
            {
                return context.SectionJobTitles
              .Where(dj => dj.SectionId == SectionId)
              .Select(dj => dj.JobTitle)
              .OrderBy(j => j.Name)
              .ToList();
            }

        }

    }
}
