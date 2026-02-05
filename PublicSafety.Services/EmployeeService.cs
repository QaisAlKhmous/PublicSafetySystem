using AdminDashboard.Services;
using PublicSafety.Domain.Entities;
using PublicSafety.Repositories;
using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
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
                EmployeeNumber = e.EmployeeNumber,
                Email = e.Email,
                Phone = e.Phone,
                IsIntern = e.IsIntern,
                Active = e.Active,
                Notes = e.Notes,
                WorkLocation = e.WorkLocation.ToString(),
                HealthInsuranceFile = e.HealthInsuranceFile,
                Department = e.Department.Name,
                Section = e.Section?.Name,
                JobTitle = e.JobTitle.Name,
                CreationDate = e.CreationDate.ToString("yyyy-MM-dd"),
                EmploymentDate = e.EmploymentDate.ToString("yyyy-MM-dd"),
                RetirementDate = e.RetirementDate?.ToString("yyyy-MM-dd"),
                Category = e.JobTitle.jobTitleCategories
                        .Select(jc => jc.Category.Name)
                        .FirstOrDefault(),
                CategoryId = e.JobTitle.jobTitleCategories
                        .Select(jc => jc.Category.CategoryId)
                        .FirstOrDefault(),
            });
        }

        public static PagedResult<EmployeePagedDTO> GetEmployeesPaged(
       int page,
       int pageSize,
       string sortField,
       string sortDir,
       Dictionary<string, string> filter
   )
        {
            using (var context = new AppDbContext())
            {

                var query = context.Employees
      .Include(e => e.Department)
      .Include(e => e.Section)
      .Include(e => e.JobTitle)
      .Select(e => new EmployeePagedDTO
      {
          EmployeeId = e.EmployeeId,

          FullName = e.FullName,
          EmployeeNumber = e.EmployeeNumber,

          Email = e.Email,
          Phone = e.Phone,

          IsIntern = e.IsIntern,
          Active = e.Active,

          Notes = e.Notes,

          WorkLocation = e.WorkLocation,
          HealthInsuranceFile = e.HealthInsuranceFile,

        
          DepartmentId = e.DepartmentId,
          Department = e.Department.Name,

         
          SectionId = e.SectionId,
          Section = e.Section != null ? e.Section.Name : null,

       
          JobTitleId = e.JobTitleId,
          JobTitle = e.JobTitle.Name,

        
          CategoryId = e.JobTitle.jobTitleCategories
              .Select(jc => jc.CategoryId)
              .FirstOrDefault(),

          Category = e.JobTitle.jobTitleCategories
              .Select(jc => jc.Category.Name)
              .FirstOrDefault(),

       
          CreationDate = e.CreationDate,

          EmploymentDate = e.EmploymentDate,

          RetirementDate = e.RetirementDate ?? DateTime.MinValue
      })
      .AsQueryable();



                if (filter != null)
                {
                    foreach (var f in filter)
                    {
                        if (string.IsNullOrWhiteSpace(f.Value))
                            continue;

                        switch (f.Key)
                        {
                            case "FullName":
                                query = query.Where(x =>
                                    x.FullName.Contains(f.Value));
                                break;

                            case "EmployeeNumber":
                                query = query.Where(x =>
                                    x.EmployeeNumber.Contains(f.Value));
                                break;

                            case "DepartmentId":
                                {
                                    Guid depId = Guid.Parse(f.Value);
                                    query = query.Where(x =>
                                        x.DepartmentId == depId);
                                    break;
                                }

                            case "SectionId":
                                {
                                    Guid secId = Guid.Parse(f.Value);
                                    query = query.Where(x =>
                                        x.SectionId == secId);
                                    break;
                                }

                            case "CategoryId":
                                {
                                    Guid catId = Guid.Parse(f.Value);
                                    query = query.Where(x =>
                                        x.CategoryId == catId);
                                    break;
                                }

                            case "Active":
                                {
                                    bool active = f.Value == "true";
                                    query = query.Where(x =>
                                        x.Active == active);
                                    break;
                                }

                            case "IsIntern":
                                {
                                    bool intern = f.Value == "true";
                                    query = query.Where(x =>
                                        x.IsIntern == intern);
                                    break;
                                }
                        }
                    }
                }

                bool asc = sortDir.Equals("asc",
                    StringComparison.OrdinalIgnoreCase);

                switch (sortField)
                {
                    case "FullName":
                        query = asc
                            ? query.OrderBy(x => x.FullName)
                            : query.OrderByDescending(x => x.FullName);
                        break;

                    case "EmployeeNumber":
                        query = asc
                            ? query.OrderBy(x => x.EmployeeNumber)
                            : query.OrderByDescending(x => x.EmployeeNumber);
                        break;

                    case "EmploymentDate":
                        query = asc
                            ? query.OrderBy(x => x.EmploymentDate)
                            : query.OrderByDescending(x => x.EmploymentDate);
                        break;

                    case "CreationDate":
                        query = asc
                            ? query.OrderBy(x => x.CreationDate)
                            : query.OrderByDescending(x => x.CreationDate);
                        break;

                    default:
                        query = asc
                            ? query.OrderBy(x => x.FullName)
                            : query.OrderByDescending(x => x.FullName);
                        break;
                }

     
                int total = query.Count();

                var data = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var item in data)
                {
                    item.EmploymentDateStr = item.EmploymentDate.ToString("yyyy-MM-dd");
                    item.WorkLocationStr = item.WorkLocation.ToString();
                }


                return new PagedResult<EmployeePagedDTO>
                {
                    Data = data,
                    Total = total
                };
            }
        }



        public static Guid AddNewEmployee(AddEmployeeDTO employee)
        {

            if (EmployeeRepo.EmployeeNumberExists(employee.EmployeeNumber))
            {
                throw new Exception("الرقم الوظيفي مستخدم مسبقاً، يرجى إدخال رقم آخر");
            }


            var newEmployee = new Employee()
            {
                EmployeeId = Guid.NewGuid(),
                EmployeeNumber = employee.EmployeeNumber,
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
                EmployeeNumber = Employee.EmployeeNumber,
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
                SectionId = Employee.Section?.SectionId,
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


            if (employee.EmployeeNumber != existingEmployee.EmployeeNumber && EmployeeRepo.EmployeeNumberExists(employee.EmployeeNumber))
            {
                throw new Exception("الرقم الوظيفي مستخدم مسبقاً، يرجى إدخال رقم آخر");
            }

            // حفظ الحالة القديمة
            bool wasActive = existingEmployee.Active;
            var oldJobTitleId = existingEmployee.JobTitleId;



            existingEmployee.EmployeeNumber = employee.EmployeeNumber;
            // تحديث البيانات الأساسية
            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.SecondName = employee.SecondName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.FullName =
                employee.FirstName + " " + employee.SecondName + " " + employee.LastName;

            existingEmployee.Email = employee.Email;
            existingEmployee.Phone = employee.Phone;
            existingEmployee.IsIntern = employee.IsIntern;
            existingEmployee.Active = employee.Active;
            existingEmployee.Notes = employee.Notes;
            existingEmployee.WorkLocation =
                (enWorkLocation)Enum.Parse(typeof(enWorkLocation), employee.WorkLocation);

            existingEmployee.HealthInsuranceFile = employee.HealthInsuranceFile;
            existingEmployee.DepartmentId = employee.DepartmentId;
            existingEmployee.SectionId = employee.SectionId;
            existingEmployee.EmploymentDate = DateTime.Parse(employee.EmploymentDate);

            EmployeeJobTitleHistory oldHistory = null;
            EmployeeJobTitleHistory newHistory = null;

            DateTime now = DateTime.Now;

            // ===============================
            // 🔁 CASE 1: Re-Activate Employee
            // ===============================
            if (!wasActive && employee.Active)
            {
                existingEmployee.RetirementDate = null;
                existingEmployee.JobTitleUpdateDate = now;
                existingEmployee.JobTitleId = employee.JobTitleId;

                newHistory = new EmployeeJobTitleHistory
                {
                    EmployeeJobTitleHistoryId = Guid.NewGuid(),
                    EmployeeId = existingEmployee.EmployeeId,
                    JobTitleId = employee.JobTitleId,
                    StartDate = now,
                    EndDate = null
                };

                return EmployeeRepo.UpdateEmployee(existingEmployee, null, newHistory);
            }

            // ===============================
            // 🔄 CASE 2: Job Title Changed (while active)
            // ===============================
            bool jobTitleChanged =
                wasActive &&
                employee.Active &&
                oldJobTitleId != employee.JobTitleId;

            if (jobTitleChanged)
            {
                existingEmployee.JobTitleId = employee.JobTitleId;
                existingEmployee.JobTitleUpdateDate = now;

                oldHistory = EmployeeJobTitleHistoryRepo
                    .GetLastJobTitleHistoryByEmployee(existingEmployee.EmployeeId);

                if (oldHistory != null)
                    oldHistory.EndDate = now;

                newHistory = new EmployeeJobTitleHistory
                {
                    EmployeeJobTitleHistoryId = Guid.NewGuid(),
                    EmployeeId = existingEmployee.EmployeeId,
                    JobTitleId = employee.JobTitleId,
                    StartDate = now,
                    EndDate = null
                };

                return EmployeeRepo.UpdateEmployee(existingEmployee, oldHistory, newHistory);
            }

            // ===============================
            // 🧘 CASE 3: Normal Update
            // ===============================
            if (wasActive && !employee.Active)
            {
                
                if (!existingEmployee.RetirementDate.HasValue)
                {
                    existingEmployee.RetirementDate = now;
                }

               
                var lastHistory = EmployeeJobTitleHistoryRepo
                    .GetLastJobTitleHistoryByEmployee(existingEmployee.EmployeeId);

                if (lastHistory != null && lastHistory.EndDate == null)
                {
                    lastHistory.EndDate = now;
                }

                return EmployeeRepo.UpdateEmployee(
                    existingEmployee,
                    lastHistory,
                    null   
                );
            }


            return EmployeeRepo.UpdateEmployee(existingEmployee, null, null);
        }


        public static bool UpdateEmployeeJobTitle(
    Guid employeeId,
    Guid newJobTitleId,
    Guid newDepartmentId,
    Guid? newSectionId
)
        {
            var employee = EmployeeRepo.GetEmployeeById(employeeId);

            if (employee == null)
                throw new Exception("الموظف غير موجود");

            if (!employee.Active)
                throw new Exception("لا يمكن تعديل بيانات موظف متقاعد");

            DateTime now = DateTime.Now;

            // ✅ If nothing changed → return
            if (employee.JobTitleId == newJobTitleId &&
                employee.DepartmentId == newDepartmentId &&
                employee.SectionId == newSectionId)
            {
                return true;
            }

            // =====================================================
            // ✅ Close previous JobTitleHistory record
            // =====================================================
            var lastHistory = EmployeeJobTitleHistoryRepo
                .GetLastJobTitleHistoryByEmployee(employeeId);

            if (lastHistory != null && lastHistory.EndDate == null)
            {
                lastHistory.EndDate = now;
            }

            // =====================================================
            // ✅ Create new JobTitleHistory
            // =====================================================
            var newHistory = new EmployeeJobTitleHistory
            {
                EmployeeJobTitleHistoryId = Guid.NewGuid(),
                EmployeeId = employeeId,
                JobTitleId = newJobTitleId,
                StartDate = now,
                EndDate = null
            };

            // =====================================================
            // ✅ Update Employee main table
            // =====================================================
            employee.JobTitleId = newJobTitleId;
            employee.JobTitleUpdateDate = now;

            employee.DepartmentId = newDepartmentId;
            employee.SectionId = newSectionId;

            // =====================================================
            // ✅ Save only those fields safely
            // =====================================================
            return EmployeeRepo.UpdateEmployee(
                employee,
                lastHistory,
                newHistory
            );
        }


        public static int GetNumberOfActiveEmployees()
        {
            return EmployeeRepo.GetNumberOfActiveEmployees();
        }
        public static int GetNumberOfInactiveEmployees()
        {
            return EmployeeRepo.GetNumberOfInactiveEmployees();
        }

        public static IEnumerable<EmployeesByCategory> GetEmployeesByCategoriesCount()
        {
            return EmployeeRepo.GetEmployeesByCategoryCount();
        }
        public static void ActivateEmployee(ActivateEmployeeDTO activateEmployee)
        {
            var employee = EmployeeRepo.GetEmployeeById(activateEmployee.EmployeeId);
            if (employee == null)
                throw new Exception("الموظف غير موجود");

            if (employee.Active)
                throw new Exception("الموظف نشط");

            var now = DateTime.Now;

            // تحديث حالة الموظف
            employee.Active = true;
            employee.RetirementDate = null;
            employee.JobTitleId = activateEmployee.JobTitleId;
            employee.DepartmentId = activateEmployee.DepartmentId;
            employee.SectionId = activateEmployee.SectionId;
            employee.JobTitleUpdateDate = DateTime.Parse(activateEmployee.ActivationDate) ;

            // إنشاء JobTitleHistory جديد
            var newHistory = new EmployeeJobTitleHistory
            {
                EmployeeJobTitleHistoryId = Guid.NewGuid(),
                EmployeeId = employee.EmployeeId,
                JobTitleId = activateEmployee.JobTitleId,
                StartDate = DateTime.Parse(activateEmployee.ActivationDate),
                EndDate = null
            };

            // تمرير البيانات جاهزة للـ Repo
            EmployeeRepo.ActivateEmployee(employee, newHistory);
        }


        public static IEnumerable<EmployeeDTO> GetEmployeesByYear(int Year)
        {
            var employees = EmployeeRepo.GetEmployeesByYear(Year);
            return employees.Select(e => new EmployeeDTO
            {
                FullName = e.FullName,
                EmployeeId = e.EmployeeId,
                CategoryId = e.JobTitle.jobTitleCategories
                        .Select(jc => jc.Category.CategoryId)
                        .FirstOrDefault(),
                Category = e.JobTitle.jobTitleCategories
                        .Select(jc => jc.Category.Name)
                        .FirstOrDefault(),
                EmployeeNumber = e.EmployeeNumber
            });

        }
    }
}
