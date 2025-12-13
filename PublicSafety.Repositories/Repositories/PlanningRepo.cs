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
        public static Dictionary<int, int> GetIssuedByYear(int fromYear, int toYear)
        {
            using (var context = new AppDbContext())
            {
                return context.Issuances
                    .Where(x =>
                        x.Type == enIssuanceType.Entitled &&
                        x.IssuanceDate.Year >= fromYear &&
                        x.IssuanceDate.Year <= toYear)
                    .GroupBy(x => x.IssuanceDate.Year)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(x => x.Quantity)
                    );
            }
        }

        public static Dictionary<int, int> GetPlannedByYear(int fromYear, int toYear)
        {
            using (var context = new AppDbContext())
            {
                // 1️⃣ Active Matrix لكل Category
                var activeMatrixByCategory = context.Matrices
                    .Where(m => m.IsActive)
                    .ToDictionary(m => m.CategoryId, m => m.MatrixId);

                // 2️⃣ MatrixItems مجمّعة حسب MatrixId
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

                // 3️⃣ الموظفون مع Category + تواريخهم
                var employees = (
                    from e in context.Employees
                    join jtc in context.JobTitleCategories
                        on e.JobTitleId equals jtc.JobTitleId
                    where e.Active
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

                // 4️⃣ الحساب سنة بسنة
                for (int year = fromYear; year <= toYear; year++)
                {
                    int totalForYear = 0;

                    foreach (var emp in employees)
                    {
                        // هل الموظف فعّال في هذه السنة؟
                        if (emp.EmploymentYear > year)
                            continue;

                        if (emp.RetirementYear.HasValue && emp.RetirementYear.Value < year)
                            continue;

                        // هل له Matrix فعّالة؟
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
                // 1️⃣ Active matrix per category
                var activeMatrixByCategory = context.Matrices
                    .Where(m => m.IsActive)
                    .ToDictionary(m => m.CategoryId, m => m.MatrixId);

                // 2️⃣ Matrix items (JOIN – no navigation properties)
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
                .ToList()   // 🔑 materialize first
                .GroupBy(x => x.MatrixId)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList()
                );

                // 3️⃣ Active employees with category
                var employees =
                (
                    from e in context.Employees
                    join jtc in context.JobTitleCategories
                        on e.JobTitleId equals jtc.JobTitleId
                    where e.Active
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

                // 4️⃣ Planning calculation
                for (int year = fromYear; year <= toYear; year++)
                {
                    foreach (var emp in employees)
                    {
                        // employee not yet hired
                        if (emp.EmploymentYear > year)
                            continue;

                        // employee retired
                        if (emp.RetirementYear.HasValue && emp.RetirementYear.Value < year)
                            continue;

                        // no active matrix for category
                        if (!activeMatrixByCategory.TryGetValue(emp.CategoryId, out Guid matrixId))
                            continue;

                        if (!matrixItemsByMatrix.TryGetValue(matrixId, out var items))
                            continue;

                        foreach (var item in items)
                        {
                            // frequency rule
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
