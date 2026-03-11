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
    public class JobTitleService
    {

        public static object AddJobTitle(AddJobTitleDTO model)
        {
            using (var db = new AppDbContext())
            {
                if (model.DepartmentId == Guid.Empty)
                    throw new Exception("يرجى اختيار الدائرة");

                if (model.CategoryId == Guid.Empty)
                    throw new Exception("يرجى اختيار الفئة");

                if (string.IsNullOrWhiteSpace(model.JobTitleName))
                    throw new Exception("يرجى إدخال اسم المسمى الوظيفي");

                var departmentExists = db.Departments.Any(x => x.DepartmentId == model.DepartmentId);
                if (!departmentExists)
                    throw new Exception("الدائرة غير موجودة");

                if (model.SectionId.HasValue)
                {
                    var sectionExists = db.Sections.Any(x =>
                        x.SectionId == model.SectionId.Value &&
                        x.DepartmentId == model.DepartmentId);

                    if (!sectionExists)
                        throw new Exception("الشعبة لا تتبع دائرة المحدد");
                }

                var categoryExists = db.Categories.Any(x => x.CategoryId == model.CategoryId);
                if (!categoryExists)
                    throw new Exception("الفئة غير موجودة");

                var normalizedInput = NormalizeArabic(model.JobTitleName);

                var existingJobTitle = db.JobTitles
                    .ToList()
                    .FirstOrDefault(x => NormalizeArabic(x.Name) == normalizedInput);

                Guid jobTitleId;

                if (existingJobTitle == null)
                {
                    var newJobTitle = new JobTitle
                    {
                        JobTitleId = Guid.NewGuid(),
                        Name = model.JobTitleName.Trim(),
                        Description = null
                    };

                    db.JobTitles.Add(newJobTitle);
                    jobTitleId = newJobTitle.JobTitleId;
                }
                else
                {
                    jobTitleId = existingJobTitle.JobTitleId;
                }

                if (model.SectionId.HasValue)
                {
                    var sectionJobTitleExists = db.SectionJobTitles.Any(x =>
                        x.SectionId == model.SectionId.Value &&
                        x.JobTitleId == jobTitleId);

                    if (!sectionJobTitleExists)
                    {
                        db.SectionJobTitles.Add(new SectionJobTitle
                        {
                            SectionJobTitleId = Guid.NewGuid(),
                            SectionId = model.SectionId.Value,
                            JobTitleId = jobTitleId
                        });
                    }
                }
                else
                {
                    var departmentJobTitleExists = db.DepartmentJobTitles.Any(x =>
                        x.DepartmentId == model.DepartmentId &&
                        x.JobTitleId == jobTitleId);

                    if (!departmentJobTitleExists)
                    {
                        db.DepartmentJobTitles.Add(new DepartmentJobTitle
                        {
                            DepartmentJobTitleId = Guid.NewGuid(),
                            DepartmentId = model.DepartmentId,
                            JobTitleId = jobTitleId
                        });
                    }
                }

                var categoryLinkExists = db.JobTitleCategories.Any(x =>
                    x.CategoryId == model.CategoryId &&
                    x.JobTitleId == jobTitleId);

                if (!categoryLinkExists)
                {
                    db.JobTitleCategories.Add(new JobTitleCategory
                    {
                        JobTitleCategoryId = Guid.NewGuid(),
                        CategoryId = model.CategoryId,
                        JobTitleId = jobTitleId
                    });
                }

                db.SaveChanges();

                return new
                {
                    JobTitleId = jobTitleId,
                    JobTitleName = model.JobTitleName
                };
            }
        }

        private static string NormalizeArabic(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return text.Trim()
                .Replace("أ", "ا")
                .Replace("إ", "ا")
                .Replace("آ", "ا")
                .Replace("ى", "ي")
                .Replace("ؤ", "و")
                .Replace("ئ", "ي")
                .Replace("ـ", "")
                .Replace(" ", "");
        }
        public static List<JobTitleListDTO> GetAllJobTitlesHierarchy()
        {
            using (var db = new AppDbContext())
            {
                var departmentLevel = (
                    from djt in db.DepartmentJobTitles
                    join d in db.Departments on djt.DepartmentId equals d.DepartmentId
                    join jt in db.JobTitles on djt.JobTitleId equals jt.JobTitleId
                    join jtc in db.JobTitleCategories on jt.JobTitleId equals jtc.JobTitleId into jtcGroup
                    from jtc in jtcGroup.DefaultIfEmpty()
                    join c in db.Categories on jtc.CategoryId equals c.CategoryId into cGroup
                    from c in cGroup.DefaultIfEmpty()
                    select new JobTitleListDTO
                    {
                        JobTitleId = jt.JobTitleId,
                        DepartmentName = d.Name,
                        SectionName = null,
                        JobTitleName = jt.Name,
                        CategoryName = c != null ? c.Name : "",
                        Level = "Department"
                    }
                ).ToList();

                var sectionLevel = (
                    from sjt in db.SectionJobTitles
                    join s in db.Sections on sjt.SectionId equals s.SectionId
                    join d in db.Departments on s.DepartmentId equals d.DepartmentId
                    join jt in db.JobTitles on sjt.JobTitleId equals jt.JobTitleId
                    join jtc in db.JobTitleCategories on jt.JobTitleId equals jtc.JobTitleId into jtcGroup
                    from jtc in jtcGroup.DefaultIfEmpty()
                    join c in db.Categories on jtc.CategoryId equals c.CategoryId into cGroup
                    from c in cGroup.DefaultIfEmpty()
                    select new JobTitleListDTO
                    {
                        JobTitleId = jt.JobTitleId,
                        DepartmentName = d.Name,
                        SectionName = s.Name,
                        JobTitleName = jt.Name,
                        CategoryName = c != null ? c.Name : "",
                        Level = "Section"
                    }
                ).ToList();

                var result = departmentLevel
                    .Concat(sectionLevel)
                    .OrderBy(x => x.DepartmentName)
                    .ThenBy(x => x.SectionName)
                    .ThenBy(x => x.JobTitleName)
                    .ThenBy(x => x.CategoryName)
                    .ToList();

                return result;
            }
        }
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

        public static JobTitleDTO GetJobTitleById(Guid Id)
        {
            var JobTitle = JobTitleRepo.GetJobTitleById(Id);

            return new JobTitleDTO() {JobTitleId = JobTitle.JobTitleId,Name= JobTitle.Name};
        }

        public static List<JobTitleDTO> GetJobTitlesByDepartment(Guid departmentId)
        {
            if (departmentId == Guid.Empty)
                return new List<JobTitleDTO>();

            var jobTitles = JobTitleRepo.GetJobTitlesByDepartment(departmentId);

            return jobTitles.Select(j => new JobTitleDTO
            {
                JobTitleId = j.JobTitleId,
                Name = j.Name
            }).ToList();
        }

        public static List<JobTitleDTO> GetJobTitlesBySection(Guid SectionId)
        {
            if (SectionId == Guid.Empty)
                return new List<JobTitleDTO>();

            var jobTitles = JobTitleRepo.GetJobTitlesBySection(SectionId);

            return jobTitles.Select(j => new JobTitleDTO
            {
                JobTitleId = j.JobTitleId,
                Name = j.Name
            }).ToList();
        }
    }
}
