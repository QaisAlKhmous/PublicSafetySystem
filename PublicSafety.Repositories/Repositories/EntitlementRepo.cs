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
        //If an item appears in multiple job titles in the same year, the employee is entitled to the MAX quantity for that year, not the sum.
        public static IEnumerable<Entitlement> GetEmployeeEntitlemenets(Guid EmployeeId,int MaxYear)
        {
            using(var context = new AppDbContext())
            {
                var employeeIdParam = new SqlParameter("@EmployeeId", EmployeeId);
                var maxYearParam = new SqlParameter("@MaxYear", MaxYear);

                return context.Database
                    .SqlQuery<Entitlement>(
                        "EXEC dbo.GetEmployeeEntitlements @EmployeeId, @MaxYear",
                        employeeIdParam,
                        maxYearParam
                    )
                    .ToList();
            }
          
        }


        public static IEnumerable<Entitlement> GetEmployeeEntitlemenetsInYear(Guid EmployeeId,int EntitlementYear,int MaxYear)
        {
            using (var context = new AppDbContext())
            {
                var employeeIdParam = new SqlParameter("@EmployeeId", EmployeeId);
                var maxYearParam = new SqlParameter("@MaxYear", MaxYear);
                return context.Database
                .SqlQuery<Entitlement>(
                        "EXEC dbo.GetEmployeeEntitlements @EmployeeId, @MaxYear",
                        employeeIdParam,
                        maxYearParam
                    )
                .Where(e => e.EntitlementYear == EntitlementYear)
                .ToList();
            }

        }
    }
}
