using PublicSafety.Domain.Entities;
using PublicSafety.Repositories;
using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
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
                var issuedByYear = PlanningRepo.GetIssuedByYear(fromYear, toYear); 
                var planned = PlanningRepo.GetPlannedByYear(fromYear, toYear);     
                var employeesPerYear = PlanningRepo.GetEmployeeCountPerYear(context, fromYear, toYear); 

                var result = new List<PlanningOverview>();

                for (int year = fromYear; year <= toYear; year++)
                {
                    int employeesCount = 0;
                    employeesPerYear.TryGetValue(year, out employeesCount);

                    int plannedValue = 0;
                    planned.TryGetValue(year, out plannedValue);

                    IssuedSummary issuedSummary;
                    if (!issuedByYear.TryGetValue(year, out issuedSummary))
                        issuedSummary = new IssuedSummary();

                    result.Add(new PlanningOverview
                    {
                        Year = year,
                        EmployeesCount = employeesCount,
                        Planned = plannedValue,
                        Issued = issuedSummary
                    });
                }

                return result;
            }
        }



        public static List<PlanningItemDetails> GetPlannedItemDetails(int fromYear, int toYear)
        {
            return PlanningRepo.GetPlannedItemsByYear(fromYear, toYear);
        }


        public static List<YearEmployeeSummaryDTO> GetYearEmployees(int year)
        {
            var employees = EmployeeService.GetEmployeesByYear(year);

            var result = new List<YearEmployeeSummaryDTO>();

            foreach (var employee in employees)
            {
                var entitlements = EntitlementService.GetEmployeeEntitlemenetsInYear(
                    employee.EmployeeId,
                    year,
                    DateTime.Now.Year + 4
                );

                if (!entitlements.Any())
                    continue;

                var issuancesCount = IssuanceService.GetNumberOfItemsIssuedInYearByEmployeeId(employee.EmployeeId, year);

                int TotalEntitled = 0;
                int TotalRemaining = 0;
                bool IsIssued = true;


                int TotalIssued = issuancesCount;
              

                if (entitlements.Any())
                {
                    TotalEntitled = entitlements.Sum(e => e.EntitledQty);
                    TotalRemaining = entitlements.Sum(e => e.RemainingQty);
                }

                if (TotalRemaining > 0)
                    IsIssued = false;

                result.Add(new YearEmployeeSummaryDTO
                {
                    EmployeeId = employee.EmployeeId,
                    EmployeeName = employee.FullName,
                    TotalEntitled = TotalEntitled,
                    TotalIssued = TotalIssued,
                    TotalRemaining = TotalRemaining,
                    Category = employee.Category,
                    CategoryId = employee.CategoryId,
                    IsIssued = IsIssued
                });
            }

            return result;
        }


    }
}
