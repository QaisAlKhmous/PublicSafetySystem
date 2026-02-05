using DocumentFormat.OpenXml.Bibliography;
using PublicSafety.Domain.Entities;
using PublicSafety.Repositories;
using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public class EntitlementService
    {

        public static IEnumerable<Entitlement> GetEntitlementsByEmployeeId(Guid EmployeeId)
        {
            return EntitlementRepo.GetEmployeeEntitlemenets(EmployeeId,DateTime.Now.Year);
        }

        public static IEnumerable<Entitlement> GetEmployeeEntitlemenetsInYear(Guid EmployeeId, int EntitlementYear, int MaxYear)
        {
            return EntitlementRepo.GetEmployeeEntitlemenetsInYear(EmployeeId, EntitlementYear, MaxYear);

        }

        public static List<EmployeeEntitlementExportRow> GetAllEmployeesEntitlementsInYear(int Year)
        {
            var employees = EmployeeService.GetAllEmployees(); 

            var result = new List<EmployeeEntitlementExportRow>();

            foreach (var emp in employees)
            {
                var entitlements =
                    EntitlementService.GetEmployeeEntitlemenetsInYear(
                        emp.EmployeeId,
                        Year,
                        Year 
                    );

                foreach (var e in entitlements)
                {
                    result.Add(new EmployeeEntitlementExportRow
                    {
                        EmployeeNumber = emp.EmployeeNumber,
                        EmployeeName = emp.FullName,

                        Department = emp.Department, 
                        Section = emp.Section,

                        Category = e.CategoryName,
                        ItemName = e.ItemName,

                        EntitledQty = e.EntitledQty,
                        IssuedQty = e.IssuedQty,
                        RemainingQty = e.RemainingQty
                    });
                }
            }

            return result;
        }

        public static PagedResult<EntitlementPaged> GetPagedEntitlements(
        int page,
        int pageSize,
        Dictionary<string, string> filter
    )
        {
            int maxYear = DateTime.Now.Year;

            // ✅ Load all entitlements once
            var data =EntitlementRepo.GetAllEntitlementsAllYears(maxYear);

            // ✅ Filters
            if (filter != null)
            {
                // ✅ Year filter
                if (filter.ContainsKey("Year") &&
                    !string.IsNullOrEmpty(filter["Year"]))
                {
                    int year = int.Parse(filter["Year"]);
                    data = data.Where(x => x.EntitlementYear == year).ToList();
                }

                // ✅ Department filter
                if (filter.ContainsKey("DepartmentId") &&
                    !string.IsNullOrEmpty(filter["DepartmentId"]))
                {
                    Guid depId = Guid.Parse(filter["DepartmentId"]);
                    data = data.Where(x => x.DepartmentId == depId).ToList();
                }

                if (filter.ContainsKey("SectionId") &&
                  !string.IsNullOrEmpty(filter["SectionId"]))
                {
                    Guid secId = Guid.Parse(filter["SectionId"]);
                    data = data.Where(x => x.SectionId == secId).ToList();
                }


                // ✅ Category filter
                if (filter.ContainsKey("CategoryId") &&
                    !string.IsNullOrEmpty(filter["CategoryId"]))
                {
                    Guid catId = Guid.Parse(filter["CategoryId"]);
                    data = data.Where(x => x.CategoryId == catId).ToList();
                }

                // ✅ Search employee
                if (filter.ContainsKey("Search") &&
                    !string.IsNullOrEmpty(filter["Search"]))
                {
                    string search = filter["Search"];

                    data = data.Where(x =>
                        x.EmployeeName.Contains(search) ||
                        x.EmployeeNumber.Contains(search)
                    ).ToList();
                }

                // ✅ IsIssued filter
                if (filter.ContainsKey("IsIssued") &&
                    !string.IsNullOrEmpty(filter["IsIssued"]))
                {
                    bool isIssued = filter["IsIssued"] == "1";

                    data = data.Where(x => x.IsIssued == isIssued).ToList();
                }

                // ✅ Remaining only
                if (filter.ContainsKey("RemainingOnly") &&
                    filter["RemainingOnly"] == "true")
                {
                    data = data.Where(x => x.RemainingQty > 0).ToList();
                }
            }

            // ✅ Total after filtering
            int total = data.Count;

            // ✅ Paging
            var paged = data
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<EntitlementPaged>
            {
                Data = paged,
                Total = total
            };
        }


    }
}
