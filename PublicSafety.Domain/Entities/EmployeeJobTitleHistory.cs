using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class EmployeeJobTitleHistory
    {
       public Guid EmployeeJobTitleHistoryId { get; set; }
       public Guid EmployeeId { get; set; }
       public Employee Employee { get; set; }
       public Guid JobTitleId { get; set; }
       public JobTitle JobTitle { get; set; }
       public DateTime StartDate {  get; set; }
       public DateTime? EndDate { get; set; }
        
    }
}
