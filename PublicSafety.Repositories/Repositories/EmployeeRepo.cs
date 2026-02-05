using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace PublicSafety.Repositories.Repositories
{
    public class EmployeeRepo
    {
        public static IEnumerable<Employee> GetAllEmployees()
        {
            using (var context = new AppDbContext())
            {
                return context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.JobTitle) 
                    .Include(e => e.JobTitle.jobTitleCategories.Select(jc => jc.Category)) 
                    .ToList();
            }
        }
        public static IEnumerable<Employee> GetAllActiveEmployees()
        {
            using (var context = new AppDbContext())
            {
                return context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.JobTitle)
                    .Include(e => e.JobTitle.jobTitleCategories.Select(jc => jc.Category))
                    .Where(e => e.Active)
                    .ToList();
            }
        }
        public static IEnumerable<Employee> GetAllInactiveEmployees()
        {
            using (var context = new AppDbContext())
            {
                return context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.JobTitle)
                    .Include(e => e.JobTitle.jobTitleCategories.Select(jc => jc.Category))
                    .Where(e => !e.Active)
                    .ToList();
            }
        }

        public static Guid AddNewEmployee(Employee newEmployee,EmployeeJobTitleHistory newEmployeeJobTitleHistory)
        {
            using (var context = new AppDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                   
                    context.Employees.Add(newEmployee);
                    context.SaveChanges(); 

                   
                   

                    context.EmployeeJobTitleHistories.Add(newEmployeeJobTitleHistory);
                    context.SaveChanges();

                   
                    transaction.Commit();

                    return newEmployee.EmployeeId;
                }
                catch
                {
                    transaction.Rollback();
                    throw; 
                }
            }
        }

        public static void ResignEmployee(Guid employeeId)
        {
            using (var context = new AppDbContext())
            {
                var employee = context.Employees
                    .FirstOrDefault(e => e.EmployeeId == employeeId);

                if (employee == null)
                    return;

                var today = DateTime.Today;

                
                employee.RetirementDate = today;
                employee.Active = false;

                
                var activeJobTitle = context.EmployeeJobTitleHistories
                    .FirstOrDefault(h =>
                        h.EmployeeId == employeeId &&
                        h.EndDate == null);

                if (activeJobTitle != null)
                {
                    activeJobTitle.EndDate = today;
                }

                context.SaveChanges();
            }
        }
        public static void ActivateEmployee(
    Employee employee,
    EmployeeJobTitleHistory newJobTitleHistory)
        {
            using (var context = new AppDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // تنظيف Navigation Properties
                    employee.JobTitle = null;
                    employee.Department = null;
                    employee.Section = null;

                    // تحديث الموظف
                    context.Employees.Attach(employee);
                    context.Entry(employee).State = EntityState.Modified;

                    // إضافة JobTitleHistory
                    context.EmployeeJobTitleHistories.Add(newJobTitleHistory);

                    context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }



        public static Employee GetEmployeeById(Guid? Id)
        {
            using(var context = new AppDbContext())
            {
                return context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.JobTitle)
                    .Include(e => e.JobTitle.jobTitleCategories.Select(jc => jc.Category))
                    .FirstOrDefault(e => e.EmployeeId == Id);
            }
        }
        public static bool UpdateEmployee(
      Employee employee,
      EmployeeJobTitleHistory oldJobTitleHistory,
      EmployeeJobTitleHistory newJobTitleHistory)
        {
            using (var context = new AppDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // مهم جدًا
                    employee.JobTitle = null;
                    employee.Department = null;
                    employee.Section = null;

                    context.Employees.Attach(employee);
                    context.Entry(employee).State = EntityState.Modified;

                    if (oldJobTitleHistory != null)
                    {
                        context.EmployeeJobTitleHistories.Attach(oldJobTitleHistory);
                        context.Entry(oldJobTitleHistory).State = EntityState.Modified;
                    }

                    if (newJobTitleHistory != null)
                    {
                        context.EmployeeJobTitleHistories.Add(newJobTitleHistory);
                    }

                    context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public static bool UpdateEmployeeJobTitleAndOrgOnly(
    Employee employee,
    EmployeeJobTitleHistory oldHistory,
    EmployeeJobTitleHistory newHistory
)
        {
            using (var context = new AppDbContext())
            using (var tx = context.Database.BeginTransaction())
            {
                try
                {
                    // ✅ Attach employee
                    context.Employees.Attach(employee);

                    // ✅ Update ONLY these fields
                    context.Entry(employee).Property(x => x.JobTitleId).IsModified = true;
                    context.Entry(employee).Property(x => x.JobTitleUpdateDate).IsModified = true;
                    context.Entry(employee).Property(x => x.DepartmentId).IsModified = true;
                    context.Entry(employee).Property(x => x.SectionId).IsModified = true;

                    // ✅ Close previous history row
                    if (oldHistory != null)
                    {
                        context.EmployeeJobTitleHistories.Attach(oldHistory);
                        context.Entry(oldHistory).Property(x => x.EndDate).IsModified = true;
                    }

                    // ✅ Insert new history row
                    if (newHistory != null)
                    {
                        context.EmployeeJobTitleHistories.Add(newHistory);
                    }

                    context.SaveChanges();
                    tx.Commit();
                    return true;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }


        public static int GetNumberOfActiveEmployees()
        {
            using (var context = new AppDbContext())
            {
                return context.Employees.Where(e => e.Active).Count();
            }
        }
        public static int GetNumberOfInactiveEmployees()
        {
            using (var context = new AppDbContext())
            {
                return context.Employees.Where(e => !e.Active).Count();
            }
        }
        public static void AddRange(List<Employee> employees)
        {
            using(var context = new AppDbContext())
            {
                context.Employees.AddRange(employees);
                context.SaveChanges();
            }
        }

        public static IEnumerable<EmployeesByCategory> GetEmployeesByCategoryCount()
        {
            using(var context = new AppDbContext())
            {
              return  context.Employees.Where(e => e.Active).Join(context.JobTitles, e => e.JobTitleId, jt => jt.JobTitleId, (e, jt) => new { e, jt })
                    .Join(context.JobTitleCategories, ej => ej.jt.JobTitleId, jtc => jtc.JobTitleId, (ej, jtc) => new { ej, jtc })
                    .Join(context.Categories, ejc => ejc.jtc.CategoryId, c => c.CategoryId, (ejc, c) => new { ejc.ej.e, c }).
                    GroupBy(x => new { x.c.CategoryId, x.c.Name })
                    .Select(g => new EmployeesByCategory {
                        CategoryId = g.Key.CategoryId,
                        CategoryName = g.Key.Name,
                        EmployeeCount = g.Select(x => x.e.EmployeeId).Distinct().Count()
                    }).ToList();
                    
                    
            }
        }



        public static IEnumerable<Employee> GetEmployeesByYear(int Year)
        {
            using(var context = new AppDbContext())
            {
                var AllEmployees = context.Employees
                    .Include(e => e.JobTitle)
                    .Include(e => e.JobTitle.jobTitleCategories.Select(jc => jc.Category))
                    .ToList();

                var employees = new List<Employee>();

               foreach(var employee in AllEmployees)
                {
                    if(employee.EmploymentDate.Year <= Year &&
                        (employee.RetirementDate == null || employee.RetirementDate.Value.Year >= Year))
                    {
                        employees.Add(employee);
                    }
                }

              

                return employees;
            }
          
        }

        public static bool EmployeeNumberExists(string employeeNumber)
        {
            using (var db = new AppDbContext())
            {
                return db.Employees.Any(e => e.EmployeeNumber == employeeNumber);
            }
        }

        public static void AddJobTitleHistoryRange(List<EmployeeJobTitleHistory> histories)
        {
            using (var db = new AppDbContext())
            {
                db.EmployeeJobTitleHistories.AddRange(histories);
                db.SaveChanges();
            }
        }


    }
}
