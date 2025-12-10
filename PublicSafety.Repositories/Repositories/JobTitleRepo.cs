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
    }
}
