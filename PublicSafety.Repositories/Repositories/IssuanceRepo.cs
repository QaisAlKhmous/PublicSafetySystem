using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
