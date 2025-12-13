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

        public static Guid? AddNewEmployee(Employee newEmployee,EmployeeJobTitleHistory newEmployeeJobTitleHistory)
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
                    // 🔴 CRITICAL FIX: clear navigation properties
                    employee.JobTitle = null;
                    employee.Department = null;
                    employee.Section = null;

                    // 1️⃣ Attach and update employee
                    context.Employees.Attach(employee);
                    context.Entry(employee).State = EntityState.Modified;

                    // 2️⃣ Close old job title history if exists
                    if (oldJobTitleHistory != null)
                    {
                        context.EmployeeJobTitleHistories.Attach(oldJobTitleHistory);
                        context.Entry(oldJobTitleHistory).State = EntityState.Modified;
                    }

                    // 3️⃣ Insert new job title history if exists
                    if (newJobTitleHistory != null)
                    {
                        context.EmployeeJobTitleHistories.Add(newJobTitleHistory);
                    }

                    // 4️⃣ Save all changes
                    context.SaveChanges();

                    // 5️⃣ Commit transaction
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
    }
}
