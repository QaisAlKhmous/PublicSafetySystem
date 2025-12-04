using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Repositories.Repositories
{
    public class DisposalRepo
    {

        public static void AddNewDisposal(Disposal disposal)
        {
            using (var context = new AppDbContext())
            {
                context.Disposals.Add(disposal);
                context.SaveChanges();
            }
        }
    }
}
