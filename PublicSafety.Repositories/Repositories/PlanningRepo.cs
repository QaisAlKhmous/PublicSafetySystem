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
    public class PlanningRepo
    {
        public static Dictionary<int, IssuedSummary> GetIssuedByYear(int fromYear, int toYear)
        {
            using (var context = new AppDbContext())
            {
                
                var issuanceData = context.Issuances
                    .Where(x =>
                        x.IssuanceDate.Year >= fromYear &&
                        x.IssuanceDate.Year <= toYear)
                    .GroupBy(x => new
                    {
                        Year = x.IssuanceDate.Year,
                        x.Type
                    })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Type,
                        Quantity = g.Sum(x => x.Quantity)
                    })
                    .ToList();

                var result = new Dictionary<int, IssuedSummary>();

                foreach (var row in issuanceData)
                {
                    if (!result.TryGetValue(row.Year, out var summary))
                    {
                        summary = new IssuedSummary();
                        result[row.Year] = summary;
                    }

                    switch (row.Type)
                    {
                        case enIssuanceType.Entitled:
                            summary.Entitled += row.Quantity;
                            break;

                        case enIssuanceType.Exception:
                            summary.Exception += row.Quantity;
                            break;

                        case enIssuanceType.Damaged:
                            summary.Damaged += row.Quantity;
                            break;
                    }

                    summary.Total += row.Quantity;
                }

                
                var disposalData = context.Disposals
                    .Where(d =>
                        d.DisposalDate.Year >= fromYear &&
                        d.DisposalDate.Year <= toYear)
                    .GroupBy(d => d.DisposalDate.Year)
                    .Select(g => new
                    {
                        Year = g.Key,
                        Quantity = g.Sum(x => x.Quantity)
                    })
                    .ToList();

                foreach (var row in disposalData)
                {
                    if (!result.TryGetValue(row.Year, out var summary))
                    {
                        summary = new IssuedSummary();
                        result[row.Year] = summary;
                    }

                    summary.Disposed += row.Quantity;
                    summary.Total += row.Quantity;
                }

                return result;
            }
        }


        public static Dictionary<int, int> GetPlannedByYear(int fromYear, int toYear)
        {
            using (var context = new AppDbContext())
            {
                var employees = context.Employees
                    .Select(e => e.EmployeeId)
                    .ToList();

                var result = new Dictionary<int, int>();

                for (int year = fromYear; year <= toYear; year++)
                {
                    int totalForYear = 0;

                    foreach (var empId in employees)
                    {
                        var employeeIdParam = new SqlParameter("@EmployeeId", empId);
                        var maxYearParam = new SqlParameter("@MaxYear", year);

                        var entitlements = context.Database.SqlQuery<Entitlement>(
                            "EXEC dbo.GetEmployeeEntitlements @EmployeeId, @MaxYear",
                            employeeIdParam,
                            maxYearParam
                        )
                        .Where(e => e.EntitlementYear == year)
                        .ToList();

                        totalForYear += entitlements.Sum(e => e.EntitledQty);
                    }

                    result[year] = totalForYear;
                }

                return result;
            }
        }






        public static Dictionary<int, int> GetEmployeeCountPerYear(
       AppDbContext context,
       int fromYear,
       int toYear)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.EmploymentDate,
                    e.RetirementDate
                })
                .ToList();

            var result = new Dictionary<int, int>();

            for (int year = fromYear; year <= toYear; year++)
            {
                // it counts the retired employee in the year of retirement
                int count = employees.Count(e =>
                    e.EmploymentDate.Year <= year &&
                    (e.RetirementDate == null || e.RetirementDate.Value.Year >= year)
                );

                result[year] = count;
            }

            return result;
        }




        public static List<PlanningItemDetails> GetPlannedItemsByYear(int fromYear, int toYear)
        {
            using (var context = new AppDbContext())
            {
                // ✅ 1. Get all employees
                var employees = context.Employees
                    .Select(e => e.EmployeeId)
                    .ToList();

                var result = new List<PlanningItemDetails>();

                // ======================================================
                // ✅ 2. Calculate Planned Qty (Entitlements)
                // ======================================================
                foreach (var empId in employees)
                {
                    var employeeParam = new SqlParameter("@EmployeeId", empId);
                    var maxYearParam = new SqlParameter("@MaxYear", toYear);

                    var entitlements = context.Database.SqlQuery<Entitlement>(
                        "EXEC dbo.GetEmployeeEntitlements @EmployeeId, @MaxYear",
                        employeeParam,
                        maxYearParam
                    )
                    .Where(e => e.EntitlementYear >= fromYear &&
                                e.EntitlementYear <= toYear)
                    .ToList();

                    foreach (var e in entitlements)
                    {
                        var existing = result.FirstOrDefault(x =>
                            x.Year == e.EntitlementYear &&
                            x.ItemId == e.ItemId
                        );

                        if (existing == null)
                        {
                            result.Add(new PlanningItemDetails
                            {
                                Year = e.EntitlementYear,
                                ItemId = e.ItemId,
                                ItemName = e.ItemName,
                                PlannedQty = e.EntitledQty,
                                IssuedQty = 0
                            });
                        }
                        else
                        {
                            existing.PlannedQty += e.EntitledQty;
                        }
                    }
                }

                // ======================================================
                // ✅ 3. Calculate Issued Qty (ALL TYPES)
                // ======================================================
                var issuedData = context.Issuances
                    .Where(i =>
                        i.IssuanceDate.Year >= fromYear &&
                        i.IssuanceDate.Year <= toYear
                    // ✅ no filter by type → includes all issuance types
                    )
                    .GroupBy(i => new
                    {
                        Year = i.IssuanceDate.Year,
                        i.ItemId
                    })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.ItemId,
                        IssuedQty = g.Sum(x => x.Quantity)
                    })
                    .ToList();

                // ======================================================
                // ✅ 4. Merge issued into result
                // ======================================================
                foreach (var row in issuedData)
                {
                    var existing = result.FirstOrDefault(x =>
                        x.Year == row.Year &&
                        x.ItemId == row.ItemId
                    );

                    if (existing == null)
                    {
                        // Item issued but not planned
                        var itemName = context.Items
                            .Where(it => it.ItemId == row.ItemId)
                            .Select(it => it.Name)
                            .FirstOrDefault();

                        result.Add(new PlanningItemDetails
                        {
                            Year = row.Year,
                            ItemId = row.ItemId,
                            ItemName = itemName,
                            PlannedQty = 0,
                            IssuedQty = row.IssuedQty
                        });
                    }
                    else
                    {
                        existing.IssuedQty = row.IssuedQty;
                    }
                }

                // ✅ Sort result
                return result
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.ItemName)
                    .ToList();
            }
        }





    }
}
