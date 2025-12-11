using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace PublicSafety.Repositories.Repositories
{
    public class IssuanceRepo
    {
        public static void AddIssuance(Issuance issuance)
        {
            using(var context = new AppDbContext())
            {
                context.Issuances.Add(issuance);
                context.SaveChanges();
            }
        }

        public static IEnumerable<Issuance> GetIssuancesByEmployeeId(Guid EmployeeId)
        {
            using(var context = new AppDbContext())
            {
                return context.Issuances.Include(i => i.Item).Include(i => i.CreatedBy).Where(i => i.EmployeeId == EmployeeId).ToList();
            }
        }
    }
}
