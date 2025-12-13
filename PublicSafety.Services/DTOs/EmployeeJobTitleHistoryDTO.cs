using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class EmployeeJobTitleHistoryDTO
    {
        public Guid EmployeeJobTitleHistoryId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid JobTitleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
