using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


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
                
                var activeMatrixByCategory = context.Matrices
                    .Where(m => m.IsActive)
                    .ToDictionary(m => m.CategoryId, m => m.MatrixId);

               
                var matrixItemsByMatrix = context.MatrixItems
                    .Where(mi => activeMatrixByCategory.Values.Contains(mi.MatrixId))
                    .GroupBy(mi => mi.MatrixId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => new
                        {
                            x.Quantity,
                            x.Frequency
                        }).ToList()
                    );

               
                var employees = (
                    from e in context.Employees
                    join jtc in context.JobTitleCategories
                        on e.JobTitleId equals jtc.JobTitleId
                    select new
                    {
                        e.EmployeeId,
                        jtc.CategoryId,
                        EmploymentYear = e.EmploymentDate.Year,
                        RetirementYear = e.RetirementDate.HasValue
                            ? e.RetirementDate.Value.Year
                            : (int?)null
                    }
                ).ToList();

                var result = new Dictionary<int, int>();

                
                for (int year = fromYear; year <= toYear; year++)
                {
                    int totalForYear = 0;

                    foreach (var emp in employees)
                    {
                      
                        if (emp.EmploymentYear > year)
                            continue;

                        if (emp.RetirementYear.HasValue && emp.RetirementYear.Value < year)
                            continue;

                        if (!activeMatrixByCategory.TryGetValue(emp.CategoryId, out Guid matrixId))
                            continue;

                        var items = matrixItemsByMatrix[matrixId]; 

                        foreach (var item in items)
                        {
                          
                            if ((year - emp.EmploymentYear) % item.Frequency == 0)
                            {
                                totalForYear += item.Quantity;
                            }
                        }
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
              
                var activeMatrixByCategory = context.Matrices
                    .Where(m => m.IsActive)
                    .ToDictionary(m => m.CategoryId, m => m.MatrixId);

              
                var matrixItemsByMatrix =
                (
                    from mi in context.MatrixItems
                    join i in context.Items on mi.ItemId equals i.ItemId
                    where activeMatrixByCategory.Values.Contains(mi.MatrixId)
                    select new
                    {
                        mi.MatrixId,
                        mi.ItemId,
                        ItemName = i.Name,
                        mi.Quantity,
                        mi.Frequency
                    }
                )
                .ToList()  
                .GroupBy(x => x.MatrixId)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList()
                );

               
                var employees =
                (
                    from e in context.Employees
                    join jtc in context.JobTitleCategories
                        on e.JobTitleId equals jtc.JobTitleId
                    select new
                    {
                        e.EmployeeId,
                        jtc.CategoryId,
                        EmploymentYear = e.EmploymentDate.Year,
                        RetirementYear = e.RetirementDate.HasValue
                            ? e.RetirementDate.Value.Year
                            : (int?)null
                    }
                ).ToList();

                var result = new List<PlanningItemDetails>();

              
                for (int year = fromYear; year <= toYear; year++)
                {
                    foreach (var emp in employees)
                    {
                      
                        if (emp.EmploymentYear > year)
                            continue;

                      
                        if (emp.RetirementYear.HasValue && emp.RetirementYear.Value < year)
                            continue;

                      
                        if (!activeMatrixByCategory.TryGetValue(emp.CategoryId, out Guid matrixId))
                            continue;

                        if (!matrixItemsByMatrix.TryGetValue(matrixId, out var items))
                            continue;

                        foreach (var item in items)
                        {
                            
                            if ((year - emp.EmploymentYear) % item.Frequency != 0)
                                continue;

                            var existing = result.FirstOrDefault(x =>
                                x.Year == year &&
                                x.ItemId == item.ItemId
                            );

                            if (existing == null)
                            {
                                result.Add(new PlanningItemDetails
                                {
                                    Year = year,
                                    ItemId = item.ItemId,
                                    ItemName = item.ItemName,
                                    PlannedQty = item.Quantity
                                });
                            }
                            else
                            {
                                existing.PlannedQty += item.Quantity;
                            }
                        }
                    }
                }

                return result;
            }
        }



    }
}
