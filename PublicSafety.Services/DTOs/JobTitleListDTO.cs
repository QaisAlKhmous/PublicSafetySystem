using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class JobTitleListDTO
    {
        public Guid JobTitleId { get; set; }
        public string DepartmentName { get; set; }
        public string SectionName { get; set; }
        public string JobTitleName { get; set; }
        public string CategoryName { get; set; }
        public string Level { get; set; }
    }
}
