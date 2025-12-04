using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public class JobTitleService
    {

        public static IEnumerable<JobTitleDTO> GetAllJobTitles()
        {
            var categories = JobTitleRepo.GetAllJobTitles();

            return categories.Select(c => new JobTitleDTO()
            {
                JobTitleId = c.JobTitleId,
                Name = c.Name,
            });
        }
        public static JobTitleDTO GetJobTitleByName(string name)
        {
            var JobTitle = JobTitleRepo.GetJobTitleByName(name);

            return new JobTitleDTO() { JobTitleId = JobTitle.JobTitleId, Name = name };
        }
    }
}
