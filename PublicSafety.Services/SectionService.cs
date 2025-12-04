using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace PublicSafety.Services
{
    public class SectionService
    {
        public static IEnumerable<SectionDTO> GetAllSections()
        {
            var categories = SectionRepo.GetAllSections();

            return categories.Select(c => new SectionDTO()
            {
                SectionId = c.SectionId,
                Name = c.Name,
            });
        }
        public static SectionDTO GetSectionByName(string name)
        {
            var Section = SectionRepo.GetSectionByName(name);

            return new SectionDTO { SectionId = Section.SectionId, Name = name };
        }
    }
}
