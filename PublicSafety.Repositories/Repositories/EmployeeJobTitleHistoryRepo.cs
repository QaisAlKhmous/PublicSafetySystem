using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Repositories.Repositories
{
    public class EmployeeJobTitleHistoryRepo
    {
        public static Guid AddNewEmployeeJobTitleHistory(EmployeeJobTitleHistory employeeJobTitleHistory)
        {
            using(var context = new AppDbContext())
            {
                context.EmployeeJobTitleHistories.Add(employeeJobTitleHistory);

                context.SaveChanges();

                return employeeJobTitleHistory.EmployeeJobTitleHistoryId;

            }
        }

        public static EmployeeJobTitleHistory GetLastJobTitleHistoryByEmployee(Guid EmployeeId)
        {
            using (var context = new AppDbContext())
            {
              return  context.EmployeeJobTitleHistories.FirstOrDefault(jh => jh.EmployeeId == EmployeeId && jh.EndDate == null);

            }
        }
    }
}
