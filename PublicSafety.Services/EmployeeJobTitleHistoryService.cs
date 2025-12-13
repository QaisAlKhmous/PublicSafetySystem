using PublicSafety.Domain.Entities;
using PublicSafety.Repositories;
using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public class EmployeeJobTitleHistoryService
    {
        public static Guid AddNewEmployeeJobTitleHistory(EmployeeJobTitleHistoryDTO employeeJobTitleHistory)
        {
            var newJobTitleHistory = new EmployeeJobTitleHistory() { EmployeeId = employeeJobTitleHistory.EmployeeId,
                EmployeeJobTitleHistoryId = Guid.NewGuid(), StartDate = employeeJobTitleHistory.StartDate, EndDate = employeeJobTitleHistory.EndDate,
                JobTitleId = employeeJobTitleHistory.JobTitleId };

           return EmployeeJobTitleHistoryRepo.AddNewEmployeeJobTitleHistory(newJobTitleHistory);
        }

        public static EmployeeJobTitleHistoryDTO GetEmployeeJobTitleHistory(Guid employeeId)
        {
            var jobTitleHIstory = EmployeeJobTitleHistoryRepo.GetLastJobTitleHistoryByEmployee(employeeId);


            return new EmployeeJobTitleHistoryDTO()
            { EmployeeId = jobTitleHIstory.EmployeeId,
                JobTitleId = jobTitleHIstory.JobTitleId,
                EmployeeJobTitleHistoryId = jobTitleHIstory.EmployeeJobTitleHistoryId,
                StartDate = jobTitleHIstory.StartDate, EndDate = jobTitleHIstory.EndDate };

          
        }
    }
}
