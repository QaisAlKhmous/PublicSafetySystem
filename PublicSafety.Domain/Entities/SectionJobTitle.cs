using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Domain.Entities
{
    public class SectionJobTitle
    {
        public Guid SectionJobTitleId { get; set; }
        public Guid SectionId { get; set; }
        public Section Section { get; set; }

        public Guid JobTitleId { get; set; }
        public JobTitle JobTitle { get; set; }
    }
}
