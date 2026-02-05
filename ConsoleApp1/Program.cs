using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using PublicSafety.Domain.Entities;
using PublicSafety.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ConsoleApp1
{
    internal class Program
    {


        public static void ImportDepartmentsFromExcel(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception("Excel file not found!");

            // ✅ Step 1: Read all department names from Column A
            List<string> departmentNames = new List<string>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheets.First();

                // Read all used rows
                foreach (var row in sheet.RowsUsed())
                {
                    string name = row.Cell(1).GetString().Trim(); // Column A

                    if (!string.IsNullOrWhiteSpace(name))
                        departmentNames.Add(name);
                }
            }

            // ✅ Step 2: Remove duplicates inside Excel itself
            departmentNames = departmentNames
                .Distinct()
                .ToList();

            // ✅ Step 3: Insert into Database (No duplicates)
            using (var context = new AppDbContext())
            {
                // Load existing department names from DB
                var existingNames = context.Departments
                    .Select(d => d.Name)
                    .ToList();

                // Get only new ones
                var newDepartments = departmentNames
                    .Except(existingNames)
                    .ToList();

                foreach (var deptName in newDepartments)
                {
                    context.Departments.Add(new PublicSafety.Domain.Entities.Department
                    {
                        DepartmentId = Guid.NewGuid(),
                        Name = deptName
                    });
                }

                context.SaveChanges();

                Console.WriteLine($"✅ Imported {newDepartments.Count} new departments successfully.");
            }
        }


        public static void ImportJobTitlesFromExcel(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception("Excel file not found!");

            // ✅ Step 1: Read all job titles from Column B
            List<string> jobTitles = new List<string>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheets.First();

                foreach (var row in sheet.RowsUsed())
                {
                    string title = row.Cell(2).GetString().Trim(); // ✅ Column B = 2

                    if (!string.IsNullOrWhiteSpace(title))
                        jobTitles.Add(title);
                }
            }

            // ✅ Step 2: Remove duplicates inside Excel
            jobTitles = jobTitles
                .Distinct()
                .ToList();

            // ✅ Step 3: Insert only new ones into DB
            using (var context = new AppDbContext())
            {
                var existingTitles = context.JobTitles
                    .Select(j => j.Name)
                    .ToList();

                var newTitles = jobTitles
                    .Except(existingTitles)
                    .ToList();

                foreach (var title in newTitles)
                {
                    context.JobTitles.Add(new JobTitle
                    {
                        JobTitleId = Guid.NewGuid(),
                        Name = title
                    });
                }

                context.SaveChanges();

                Console.WriteLine($"✅ Imported {newTitles.Count} new job titles successfully.");
            }
        }

        public static void ImportJobTitleCategories(string filePath)
        {


            Console.OutputEncoding = Encoding.UTF8;
            if (!File.Exists(filePath))
                throw new Exception("Excel file not found!");

            using (var context = new AppDbContext())
            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheets.First();

                foreach (var row in sheet.RowsUsed())
                {
                    // ✅ Column 2 = JobTitle
                    string jobTitleName = row.Cell(2).GetString().Trim();

                    // ✅ Column 3 = Category
                    string categoryName = row.Cell(3).GetString().Trim().Replace("إ", "ا");

                    if (string.IsNullOrWhiteSpace(jobTitleName) ||
                        string.IsNullOrWhiteSpace(categoryName))
                    {
                        System.Diagnostics.Debug.WriteLine(categoryName + jobTitleName);
                    }

                    // ✅ 1. Find JobTitle in DB
                    var jobTitle = context.JobTitles
                        .FirstOrDefault(j => j.Name == jobTitleName);

                    if (jobTitle == null)
                    {
                        System.Diagnostics.Debug.WriteLine(jobTitleName);
                        continue; // Or you can auto-create it
                    }
                      

                    // ✅ 2. Find Category in DB
                    var category = context.Categories.ToList()
                        .FirstOrDefault(c => c.Name.Replace("إ", "ا") == categoryName);

                    if (category == null)
                    {
                        System.Diagnostics.Debug.WriteLine(categoryName);
                        continue; // Or you can auto-create it
                    }

                    // ✅ 3. Check if relation already exists
                    bool exists = context.JobTitleCategories.Any(x =>
                        x.JobTitleId == jobTitle.JobTitleId &&
                        x.CategoryId == category.CategoryId);

                    if (!exists)
                    {
                        context.JobTitleCategories.Add(new JobTitleCategory
                        {
                            JobTitleCategoryId = Guid.NewGuid(),
                            JobTitleId = jobTitle.JobTitleId,
                            CategoryId = category.CategoryId
                        });
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(categoryName + jobTitleName);
                    }
                }

                context.SaveChanges();

                Console.WriteLine("✅ JobTitleCategories imported successfully.");
            }
        }




   
        public static void ImportDepartmentsAndSections(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception("Excel file not found!");

            // ✅ Step 1: Read all Department + Section pairs
            var pairs = new List<(string Dept, string Section)>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheets.First();

                foreach (var row in sheet.RowsUsed())
                {
                    string deptName = (row.Cell(1).GetString());
                    string sectionName = (row.Cell(2).GetString());

                    if (string.IsNullOrWhiteSpace(deptName) ||
                        string.IsNullOrWhiteSpace(sectionName))
                        continue;

                    pairs.Add((deptName, sectionName));
                }
            }

            // ✅ Step 2: Remove duplicates from Excel
            pairs = pairs.Distinct().ToList();

            // ✅ Step 3: Insert into DB safely
            using (var context = new AppDbContext())
            {
                foreach (var pair in pairs)
                {
                    // Department
                    var department = context.Departments
                        .FirstOrDefault(d => d.Name == pair.Dept);

                    if (department == null)
                    {
                        department = new PublicSafety.Domain.Entities.Department
                        {
                            DepartmentId = Guid.NewGuid(),
                            Name = pair.Dept
                        };

                        context.Departments.Add(department);
                        context.SaveChanges();
                    }

                    // Section
                    bool exists = context.Sections.Any(s =>
                        s.DepartmentId == department.DepartmentId &&
                        s.Name == pair.Section);

                    if (!exists)
                    {
                        context.Sections.Add(new Section
                        {
                            SectionId = Guid.NewGuid(),
                            Name = pair.Section,
                            DepartmentId = department.DepartmentId
                        });
                    }
                }

                context.SaveChanges();
            }

            Console.WriteLine("✅ Departments + Sections imported without duplicates!");

        }



        public static void ImportJobTitlesWithLinks(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception("Excel file not found!");

            Debug.WriteLine("✅ Starting ImportJobTitlesWithLinks...");
            Debug.WriteLine("📌 File Path: " + filePath);

            using (var context = new AppDbContext())
            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheets.First();

                foreach (var row in sheet.RowsUsed())
                {
                    string deptName = (row.Cell(1).GetString());
                    string sectionName = (row.Cell(2).GetString());
                    string jobTitleName =  (row.Cell(3).GetString());

                    Debug.WriteLine("--------------------------------------------------");
                    Debug.WriteLine($"📍 Row Data => Dept: [{deptName}], Section: [{sectionName}], JobTitle: [{jobTitleName}]");

                    if (string.IsNullOrWhiteSpace(deptName) ||
                        string.IsNullOrWhiteSpace(jobTitleName))
                    {
                        Debug.WriteLine("⚠️ Skipped row بسبب Department أو JobTitle فارغ.");
                        continue;
                    }

                    // ✅ 1. Department
                    var department = context.Departments.ToList()
                        .FirstOrDefault(d => d.Name  == deptName);

                    if (department == null)
                    {
                        Debug.WriteLine($"➕ Creating Department: {deptName}");

                        department = new PublicSafety.Domain.Entities.Department
                        {
                            DepartmentId = Guid.NewGuid(),
                            Name = deptName
                        };

                        context.Departments.Add(department);
                        context.SaveChanges();
                    }
                    else
                    {
                        Debug.WriteLine($"✅ Department exists: {deptName}");
                    }

                    // ✅ 2. JobTitle
                    var jobTitle = context.JobTitles.ToList()
                        .FirstOrDefault(j => j.Name  == jobTitleName);

                    if (jobTitle == null)
                    {
                        Debug.WriteLine($"➕ Creating JobTitle: {jobTitleName}");

                        jobTitle = new JobTitle
                        {
                            JobTitleId = Guid.NewGuid(),
                            Name = jobTitleName
                        };

                        context.JobTitles.Add(jobTitle);
                        context.SaveChanges();
                    }
                    else
                    {
                        Debug.WriteLine($"✅ JobTitle exists: {jobTitleName}");
                    }

                    // ✅ الحالة 1: يوجد Section
                    if (!string.IsNullOrWhiteSpace(sectionName))
                    {
                        Debug.WriteLine($"📌 Section موجود → Linking JobTitle to Section: {sectionName}");

                        var section = context.Sections.ToList()
                            .FirstOrDefault(s =>
                               s.Name == sectionName &&
                                s.DepartmentId == department.DepartmentId);

                        if (section == null)
                        {
                            Debug.WriteLine($"➕ Creating Section: {sectionName}");

                            section = new Section
                            {
                                SectionId = Guid.NewGuid(),
                                Name = sectionName,
                                DepartmentId = department.DepartmentId
                            };

                            context.Sections.Add(section);
                            context.SaveChanges();
                        }
                        else
                        {
                            Debug.WriteLine($"✅ Section exists: {sectionName}");
                        }

                        // ✅ Link Section ↔ JobTitle
                        bool exists = context.SectionJobTitles.Any(x =>
                            x.SectionId == section.SectionId &&
                            x.JobTitleId == jobTitle.JobTitleId);

                        if (!exists)
                        {
                            Debug.WriteLine("➕ Adding SectionJobTitle link...");

                            context.SectionJobTitles.Add(new SectionJobTitle
                            {
                                SectionJobTitleId = Guid.NewGuid(),
                                SectionId = section.SectionId,
                                JobTitleId = jobTitle.JobTitleId
                            });
                        }
                        else
                        {
                            Debug.WriteLine("✅ SectionJobTitle link already exists.");
                        }
                    }

                    // ✅ الحالة 2: لا يوجد Section
                    else
                    {
                        Debug.WriteLine($"📌 No Section → Linking JobTitle directly to Department: {deptName}");

                        bool exists = context.DepartmentJobTitles.Any(x =>
                            x.DepartmentId == department.DepartmentId &&
                            x.JobTitleId == jobTitle.JobTitleId);

                        if (!exists)
                        {
                            Debug.WriteLine("➕ Adding DepartmentJobTitle link...");

                            context.DepartmentJobTitles.Add(new DepartmentJobTitle
                            {
                                DepartmentJobTitleId = Guid.NewGuid(),
                                DepartmentId = department.DepartmentId,
                                JobTitleId = jobTitle.JobTitleId
                            });
                        }
                        else
                        {
                            Debug.WriteLine("✅ DepartmentJobTitle link already exists.");
                        }
                    }
                }

                context.SaveChanges();
            }

            Debug.WriteLine("✅ Import Finished Successfully!");
        }


        public static string Clean(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            return text.Trim()
                .Replace("\u00A0", " ")
                .Replace("أ", "ا")
                .Replace("إ", "ا")
                .Replace("آ", "ا")
                .Replace("ى", "ي")
                .Replace("ة", "ه")
                .Replace("  ", " ");
        }

        public static void ImportEverything(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception("Excel file not found!");

            Debug.WriteLine("✅ Starting Full Import...");

            using (var context = new AppDbContext())
            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheets.First();

                foreach (var row in sheet.RowsUsed())
                {
                    // ✅ Read Original Values (Stored As-Is)
                    string deptOriginal = row.Cell(1).GetString().Trim();
                    string sectionOriginal = row.Cell(2).GetString().Trim();
                    string jobTitleOriginal = row.Cell(3).GetString().Trim();
                    string categoryOriginal = row.Cell(4).GetString().Trim();

                    // ✅ Clean Values (Only For Comparison)
                    string deptClean = Clean(deptOriginal);
                    string sectionClean = Clean(sectionOriginal);
                    string jobTitleClean = Clean(jobTitleOriginal);
                    string categoryClean = Clean(categoryOriginal);

                    Debug.WriteLine("--------------------------------------------------");
                    Debug.WriteLine($"📍 Row => Dept: [{deptOriginal}], Section: [{sectionOriginal}], Job: [{jobTitleOriginal}], Cat: [{categoryOriginal}]");

                    if (string.IsNullOrWhiteSpace(deptOriginal) ||
                        string.IsNullOrWhiteSpace(jobTitleOriginal))
                    {
                        Debug.WriteLine("⚠️ Skipped row بسبب نقص Department أو JobTitle");
                        continue;
                    }

                    // =====================================================
                    // ✅ 1. Department (Find by Clean, Store Original)
                    // =====================================================
                    var department = context.Departments
                        .ToList()
                        .FirstOrDefault(d => Clean(d.Name) == deptClean);

                    if (department == null)
                    {
                        Debug.WriteLine($"➕ Creating Department: {deptOriginal}");

                        department = new PublicSafety.Domain.Entities.Department
                        {
                            DepartmentId = Guid.NewGuid(),
                            Name = deptOriginal
                        };

                        context.Departments.Add(department);
                        context.SaveChanges();
                    }

                    // =====================================================
                    // ✅ 2. Section (Optional)
                    // =====================================================
                    Section section = null;

                    if (!string.IsNullOrWhiteSpace(sectionOriginal))
                    {
                        section = context.Sections
                            .ToList()
                            .FirstOrDefault(s =>
                                s.DepartmentId == department.DepartmentId &&
                                Clean(s.Name) == sectionClean);

                        if (section == null)
                        {
                            Debug.WriteLine($"➕ Creating Section: {sectionOriginal}");

                            section = new Section
                            {
                                SectionId = Guid.NewGuid(),
                                Name = sectionOriginal,
                                DepartmentId = department.DepartmentId
                            };

                            context.Sections.Add(section);
                            context.SaveChanges();
                        }
                    }

                    // =====================================================
                    // ✅ 3. JobTitle
                    // =====================================================
                    var jobTitle = context.JobTitles
                        .ToList()
                        .FirstOrDefault(j => Clean(j.Name) == jobTitleClean);

                    if (jobTitle == null)
                    {
                        Debug.WriteLine($"➕ Creating JobTitle: {jobTitleOriginal}");

                        jobTitle = new JobTitle
                        {
                            JobTitleId = Guid.NewGuid(),
                            Name = jobTitleOriginal
                        };

                        context.JobTitles.Add(jobTitle);
                        context.SaveChanges();
                    }

                    // =====================================================
                    // ✅ 4. Category
                    // =====================================================
                    Category category = null;

                    if (!string.IsNullOrWhiteSpace(categoryOriginal))
                    {
                        category = context.Categories
                            .ToList()
                            .FirstOrDefault(c => Clean(c.Name) == categoryClean);

                        if (category == null)
                        {
                            Debug.WriteLine($"➕ Creating Category: {categoryOriginal}");

                            category = new Category
                            {
                                CategoryId = Guid.NewGuid(),
                                Name = categoryOriginal
                            };

                            context.Categories.Add(category);
                            context.SaveChanges();
                        }

                        // ✅ Link JobTitle ↔ Category
                        bool catLinkExists = context.JobTitleCategories.Any(x =>
                            x.JobTitleId == jobTitle.JobTitleId &&
                            x.CategoryId == category.CategoryId);

                        if (!catLinkExists)
                        {
                            Debug.WriteLine("➕ Linking JobTitle to Category...");

                            context.JobTitleCategories.Add(new JobTitleCategory
                            {
                                JobTitleCategoryId = Guid.NewGuid(),
                                JobTitleId = jobTitle.JobTitleId,
                                CategoryId = category.CategoryId
                            });
                        }
                    }

                    // =====================================================
                    // ✅ 5. Link JobTitle ↔ Section or Department
                    // =====================================================

                    if (section != null)
                    {
                        bool sectionLinkExists = context.SectionJobTitles.Any(x =>
                            x.SectionId == section.SectionId &&
                            x.JobTitleId == jobTitle.JobTitleId);

                        if (!sectionLinkExists)
                        {
                            Debug.WriteLine("➕ Linking JobTitle to Section...");

                            context.SectionJobTitles.Add(new SectionJobTitle
                            {
                                SectionJobTitleId = Guid.NewGuid(),
                                SectionId = section.SectionId,
                                JobTitleId = jobTitle.JobTitleId
                            });
                        }
                    }
                    else
                    {
                        bool deptLinkExists = context.DepartmentJobTitles.Any(x =>
                            x.DepartmentId == department.DepartmentId &&
                            x.JobTitleId == jobTitle.JobTitleId);

                        if (!deptLinkExists)
                        {
                            Debug.WriteLine("➕ Linking JobTitle to Department...");

                            context.DepartmentJobTitles.Add(new DepartmentJobTitle
                            {
                                DepartmentJobTitleId = Guid.NewGuid(),
                                DepartmentId = department.DepartmentId,
                                JobTitleId = jobTitle.JobTitleId
                            });
                        }
                    }
                }

                // ✅ Save Everything Once
                context.SaveChanges();
            }

            Debug.WriteLine("✅ Full Import Completed Successfully!");
        }
        static void Main(string[] args)
        {

            ImportEverything("C:\\Users\\qaisk\\OneDrive\\Documents\\Downloads\\JobTitlesWithSections.xlsx");
        }
    }
}
