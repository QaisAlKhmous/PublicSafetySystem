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
                Name = e.Name,
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



        public static Guid AddNewEmployee(AddEmployeeDTO employee)
        {
            var newEmployee = new Employee()
            {
                EmployeeId = Guid.NewGuid(),
                Name = employee.Name,
                Email = employee.Email,
                Phone = employee.Phone,
                IsIntern = employee.IsIntern,
                EmploymentDate = DateTime.Parse(employee.EmploymentDate),
                SectionId = employee.SectionId,
                DepartmentId = employee.DepartmentId,
                JobTitleId = employee.JobTitleId,
                Active = true,
                Notes = employee.Notes,
                CreationDate = DateTime.Now,
                WorkLocation = (enWorkLocation)Enum.Parse(typeof(enWorkLocation), employee.WorkLocation),
                HealthInsuranceFile = employee.HealthInsuranceFile
            };

            return EmployeeRepo.AddNewEmployeee(newEmployee);
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
                Name = Employee.Name,
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
                EmploymentDate = Employee.EmploymentDate.ToString()
            };
        }

        public static bool UpdateEmployee(AddEmployeeDTO employee)
        {
            var existingEmployee = EmployeeRepo.GetEmployeeById(employee.EmployeeId);

            if (existingEmployee == null)
                return false;


            existingEmployee.Name = employee.Name;
            existingEmployee.Email = employee.Email;
            existingEmployee.Phone = employee.Phone;
            existingEmployee.IsIntern = employee.IsIntern;
            existingEmployee.Active = employee.Active;
            existingEmployee.Notes = employee.Notes;
            existingEmployee.WorkLocation = (enWorkLocation)Enum.Parse(typeof(enWorkLocation), employee.WorkLocation);
            existingEmployee.HealthInsuranceFile = employee.HealthInsuranceFile;
            existingEmployee.DepartmentId = employee.DepartmentId;
            existingEmployee.HealthInsuranceFile = employee.HealthInsuranceFile;
            existingEmployee.SectionId = employee.SectionId;
            existingEmployee.JobTitleId = employee.JobTitleId;
            existingEmployee.EmploymentDate = DateTime.Parse(employee.EmploymentDate);

            // TO DO -- Update CategoryId


            return EmployeeRepo.UpdateEmployee(existingEmployee);

        }
    }
}
