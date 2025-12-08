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

        public static Guid AddNewEmployeee(Employee newEmployee)
        {
            using (var context = new AppDbContext())
            {
                context.Employees.Add(newEmployee);
                context.SaveChanges();

                return newEmployee.EmployeeId;
            }
        }

        public static void ResignEmployee(Guid EmployeeId)
        {
            using (var context = new AppDbContext())
            {
                var Employee = context.Employees.FirstOrDefault(e => e.EmployeeId == EmployeeId);

                Employee.RetirementDate = DateTime.Now;
                Employee.Active = false;

                context.SaveChanges();
            }

        }

        public static Employee GetEmployeeById(Guid Id)
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

        public static bool UpdateEmployee(Employee updatedEmployee)
        {
            using (var context = new AppDbContext())
            {
                // Load existing employee
                var employee = context.Employees.Find(updatedEmployee.EmployeeId);
                if (employee == null) return false;

                // Update only the properties you want
                employee.FirstName = updatedEmployee.FirstName;
                employee.SecondName = updatedEmployee.SecondName;
                employee.LastName = updatedEmployee.LastName;
                employee.FullName = updatedEmployee.FullName;
                employee.Email = updatedEmployee.Email;
                employee.Phone = updatedEmployee.Phone;
                employee.Active = updatedEmployee.Active;
                employee.IsIntern = updatedEmployee.IsIntern;
                employee.WorkLocation = updatedEmployee.WorkLocation;
                employee.Notes = updatedEmployee.Notes;
                employee.HealthInsuranceFile = updatedEmployee.HealthInsuranceFile;
                employee.EmploymentDate = updatedEmployee.EmploymentDate;

                // Update FKs only, navigation properties will resolve automatically
                employee.DepartmentId = updatedEmployee.DepartmentId;
                employee.SectionId = updatedEmployee.SectionId;
                employee.JobTitleId = updatedEmployee.JobTitleId;

                // Optional: update RetirementDate, etc.
                employee.RetirementDate = updatedEmployee.RetirementDate;

                context.SaveChanges();
                return true;
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
