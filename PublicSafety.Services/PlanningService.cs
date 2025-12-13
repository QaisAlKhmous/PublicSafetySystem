using PublicSafety.Domain.Entities;
using PublicSafety.Repositories;
using PublicSafety.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public class PlanningService
    {
        public static List<PlanningOverview> GetOverview(int fromYear, int toYear)
        {
            using (var context = new AppDbContext())
            {
                var issued = PlanningRepo.GetIssuedByYear(fromYear, toYear);

                var planned = PlanningRepo.GetPlannedByYear(fromYear, toYear);

                var employeesPerYear =
                    PlanningRepo.GetEmployeeCountPerYear(context, fromYear, toYear);

                var result = new List<PlanningOverview>();

                for (int year = fromYear; year <= toYear; year++)
                {
                    result.Add(new PlanningOverview
                    {
                        Year = year,
                        EmployeesCount = employeesPerYear.ContainsKey(year)
                            ? employeesPerYear[year]
                            : 0,

                        Issued = issued.ContainsKey(year)
                            ? issued[year]
                            : 0,

                        Planned = planned.ContainsKey(year)
                            ? planned[year]
                            : 0
                    });
                }

                return result;
            }
        }

        public static List<PlanningItemDetails> GetPlannedItemDetails(int fromYear, int toYear)
        {
            return PlanningRepo.GetPlannedItemsByYear(fromYear, toYear);
        }

    }
}
