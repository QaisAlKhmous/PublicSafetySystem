using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Repositories.Repositories
{
    public class EntitlementRepo
    {
      public static IEnumerable<Entitlement> GetEmployeeEntitlemenets(Guid EmployeeId)
        {
            using(var context = new AppDbContext())
            {
                var employeeIdParam = new SqlParameter("@EmployeeId", EmployeeId);
                return context.Database
                .SqlQuery<Entitlement>("EXEC dbo.GetEmployeeEntitlements @EmployeeId", employeeIdParam)
                .ToList();
            }
          
        }
    }
}
