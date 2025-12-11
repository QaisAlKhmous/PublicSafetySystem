using PublicSafety.Domain.Entities;
using PublicSafety.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public class EntitlementService
    {

        public static IEnumerable<Entitlement> GetEntitlementsByEmployeeId(Guid EmployeeId)
        {
            return EntitlementRepo.GetEmployeeEntitlemenets(EmployeeId);
        }
    }
}
