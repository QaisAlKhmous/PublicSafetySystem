using AdminDashboard.Services;
using PublicSafety.Domain.Entities;
using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public class EmployeeService
    {

        public static IEnumerable<EmployeeDTO> GetAllEmployees()
        {
            var Employees = EmployeeRepo.GetAllEmployees();

            return Employees.Select(e => new EmployeeDTO
            {
                EmployeeId = e.EmployeeId,
                FullName = e.FullName,
                Email = e.Email,
                Phone = e.Phone,
                IsIntern = e.IsIntern,
                Active = e.Active,
                Notes = e.Notes,
                WorkLocation = e.WorkLocation.ToString(),
                HealthInsuranceFile = e.HealthInsuranceFile,
                Department = e.Department.Name,
                Section = e.Section.Name,
                JobTitle = e.JobTitle.Name,
                CreationDate = e.CreationDate.ToString("yyyy-MM-dd"),
                EmploymentDate = e.EmploymentDate.ToString("yyyy-MM-dd"),
                RetirementDate = e.RetirementDate?.ToString("yyyy-MM-dd"),
                Category = e.JobTitle.jobTitleCategories
                        .Select(jc => jc.Category.Name)
                        .FirstOrDefault()
            });
        }



        public static Guid? AddNewEmployee(AddEmployeeDTO employee)
        {
            var newEmployee = new Employee()
            {
                EmployeeId = Guid.NewGuid(),
                FirstName = employee.FirstName,
                SecondName = employee.SecondName,
                LastName = employee.LastName,
                FullName = employee.FullName = employee.FirstName + " " + employee.SecondName + " " + employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                IsIntern = employee.IsIntern,
                EmploymentDate = DateTime.Parse(employee.EmploymentDate),
                SectionId = employee.SectionId,
                DepartmentId = employee.DepartmentId,
                JobTitleId = employee.JobTitleId,
                Active = employee.Active,
                Notes = employee.Notes,
                CreationDate = DateTime.Now,
                WorkLocation = (enWorkLocation)Enum.Parse(typeof(enWorkLocation), employee.WorkLocation),
                HealthInsuranceFile = employee.HealthInsuranceFile,
                JobTitleUpdateDate = DateTime.Parse(employee.EmploymentDate)
            };

            var JobTitleHistory = new EmployeeJobTitleHistory()
            {
                EmployeeId = newEmployee.EmployeeId,
                EmployeeJobTitleHistoryId = Guid.NewGuid(),
                JobTitleId = newEmployee.JobTitleId,
                StartDate = newEmployee.JobTitleUpdateDate,
                EndDate = null
            };

            return EmployeeRepo.AddNewEmployee(newEmployee,JobTitleHistory);
        }

        public static void ResignEmployee(Guid EmployeeId)
        {
            EmployeeRepo.ResignEmployee(EmployeeId);
        }

        public static AddEmployeeDTO GetEmployeeById(Guid Id)
        {
            var Employee = EmployeeRepo.GetEmployeeById(Id);

            return new AddEmployeeDTO()
            {
                EmployeeId = Employee.EmployeeId,
                FullName = Employee.FullName,
                FirstName = Employee.FirstName,
                SecondName = Employee.SecondName,
                LastName = Employee.LastName,
                Email = Employee.Email,
                Phone = Employee.Phone,
                IsIntern = Employee.IsIntern,
                Active = Employee.Active,
                Notes = Employee.Notes,
                WorkLocation = Employee.WorkLocation.ToString(),
                HealthInsuranceFile = Employee.HealthInsuranceFile,
                DepartmentId = Employee.Department.DepartmentId,
                SectionId = Employee.Section.SectionId,
                JobTitleId = Employee.JobTitle.JobTitleId,
                CategoryId = Employee.JobTitle.jobTitleCategories.Select(jc => jc.Category.CategoryId).FirstOrDefault(),
                EmploymentDate = Employee.EmploymentDate.ToString(),
                JobTitleUpdateDate = Employee.JobTitleUpdateDate.ToString()
            };
        }

        public static bool UpdateEmployee(AddEmployeeDTO employee)
        {
            var existingEmployee = EmployeeRepo.GetEmployeeById(employee.EmployeeId);
            if (existingEmployee == null)
                return false;

            // ✅ Save old JobTitleId BEFORE changing it
            var oldJobTitleId = existingEmployee.JobTitleId;

            // Update normal fields
            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.SecondName = employee.SecondName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.FullName = employee.FirstName + " " + employee.SecondName + " " + employee.LastName;
            existingEmployee.Email = employee.Email;
            existingEmployee.Phone = employee.Phone;
            existingEmployee.IsIntern = employee.IsIntern;
            existingEmployee.Active = employee.Active;
            existingEmployee.Notes = employee.Notes;
            existingEmployee.WorkLocation = (enWorkLocation)Enum.Parse(typeof(enWorkLocation), employee.WorkLocation);
            existingEmployee.HealthInsuranceFile = employee.HealthInsuranceFile;
            existingEmployee.DepartmentId = employee.DepartmentId;
            existingEmployee.SectionId = employee.SectionId;
            existingEmployee.EmploymentDate = DateTime.Parse(employee.EmploymentDate);


            if (existingEmployee.Active)
                existingEmployee.RetirementDate = null;

            bool jobTitleChanged = oldJobTitleId != employee.JobTitleId;

            if (jobTitleChanged)
            {
                DateTime changeDate = DateTime.Now;

                existingEmployee.JobTitleId = employee.JobTitleId;
                existingEmployee.JobTitleUpdateDate = changeDate;

                var jobTitleHistory = EmployeeJobTitleHistoryRepo
                    .GetLastJobTitleHistoryByEmployee(existingEmployee.EmployeeId);

                jobTitleHistory.EndDate = changeDate;

                var newJobTitleHistory = new EmployeeJobTitleHistory
                {
                    EmployeeJobTitleHistoryId = Guid.NewGuid(),
                    EmployeeId = existingEmployee.EmployeeId,
                    JobTitleId = employee.JobTitleId,
                    StartDate = changeDate,
                    EndDate = null
                };

                return EmployeeRepo.UpdateEmployee(
                    existingEmployee,
                    jobTitleHistory,
                    newJobTitleHistory);
            }

            return EmployeeRepo.UpdateEmployee(existingEmployee, null, null);
        }
 

        public static int GetNumberOfActiveEmployees()
        {
            return EmployeeRepo.GetNumberOfActiveEmployees();
        }
        public static int GetNumberOfInactiveEmployees()
        {
            return EmployeeRepo.GetNumberOfInactiveEmployees();
        }

    }
}
