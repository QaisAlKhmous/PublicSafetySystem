using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using PublicSafety.Domain.Entities;
using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public class ExcelService
    {

        public static ExcelUploadResult AddEmployeesFromExcel(Stream fileStream)
        {
            List<string> errors = new List<string>();
            List<Employee> employees = new List<Employee>();

            using (var wb = new XLWorkbook(fileStream))
            {
                var ws = wb.Worksheet(1);
                var rows = ws.RangeUsed().RowsUsed();

                foreach (var row in rows.Skip(1)) // skip header
                {
                    try
                    {
                        string firstName = row.Cell(1).GetString().Trim();
                        string secondName = row.Cell(2).GetString().Trim();
                        string lastName = row.Cell(3).GetString().Trim();
                        string email = row.Cell(4).GetString().Trim();
                        string phone = row.Cell(5).GetString().Trim();

                        DateTime employmentDate = row.Cell(6).GetDateTime();
                        string workLocationStr = row.Cell(7).GetString().Trim();

                        string departmentName = row.Cell(8).GetString().Trim();
                        string sectionName = row.Cell(9).GetString().Trim();
                        string jobTitleName = row.Cell(10).GetString().Trim();

                        if (!Enum.TryParse(workLocationStr, true, out enWorkLocation workLocation))
                        {
                            errors.Add($"Work location invalid at row {row.RowNumber()}.");
                            continue;
                        }

                        var dept = DepartmentService.GetDepartmentByName(departmentName);
                        if (dept == null)
                        {
                            errors.Add($"Invalid department '{departmentName}' at row {row.RowNumber()}.");
                            continue;
                        }

                        var section = SectionService.GetSectionByName(sectionName);
                        if (section == null)
                        {
                            errors.Add($"Invalid section '{sectionName}' at row {row.RowNumber()}.");
                            continue;
                        }

                        var jobTitle = JobTitleService.GetJobTitleByName(jobTitleName);
                        if (jobTitle == null)
                        {
                            errors.Add($"Invalid job title '{jobTitleName}' at row {row.RowNumber()}.");
                            continue;
                        }

                        employees.Add(new Employee
                        {
                            EmployeeId = Guid.NewGuid(),
                            FullName = firstName + " " + secondName + " " + lastName,
                            FirstName = firstName,
                            SecondName = secondName,
                            LastName = lastName,
                            Email = email,
                            Phone = phone,
                            EmploymentDate = employmentDate,
                            WorkLocation = workLocation,
                            DepartmentId = dept.DepartmentId,
                            SectionId = section.SectionId,
                            JobTitleId = jobTitle.JobTitleId,
                            IsIntern = true,
                            Active = true,
                            JobTitleUpdateDate = employmentDate,
                            CreationDate =DateTime.Now
                        });
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {row.RowNumber()}: {ex.Message}");
                    }
                }
            }

            EmployeeRepo.AddRange(employees);

            return new ExcelUploadResult
            {
                SuccessCount = employees.Count,
                Errors = errors
            };
        }


        public static byte[] GenerateEmployeeTemplate()
        {
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Employees");

                ws.Cell("A1").Value = "First Name";
                ws.Cell("B1").Value = "Second Name";
                ws.Cell("C1").Value = "Last Name";
                ws.Cell("D1").Value = "Email";
                ws.Cell("E1").Value = "Phone";
                ws.Cell("F1").Value = "Employment Date (yyyy-mm-dd)";
                ws.Cell("G1").Value = "Work Location (Amman/Khaldieh)";
                ws.Cell("H1").Value = "Department Name";
                ws.Cell("I1").Value = "Section Name";
                ws.Cell("J1").Value = "Job Title";

                ws.Range("A1:J1").Style.Font.Bold = true;

                ws.Column(1).Width = 20;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 20;
                ws.Column(5).Width = 20;
                ws.Column(6).Width = 30;
                ws.Column(7).Width = 30;
                ws.Column(8).Width = 20;
                ws.Column(9).Width = 20;
                ws.Column(10).Width = 20;

                using (var ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    return ms.ToArray();
                }
            }
        }

        public static byte[] ExportAllEmployeesToExcel()
        {
            var employees = EmployeeRepo.GetAllActiveEmployees(); // fetch all employees

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Employees");

                // Header row
                ws.Cell(1, 1).Value = "First Name";
                ws.Cell(1, 2).Value = "Second Name";
                ws.Cell(1, 3).Value = "Last Name";
                ws.Cell(1, 4).Value = "Email";
                ws.Cell(1, 5).Value = "Phone";
                ws.Cell(1, 6).Value = "Employment Date";
                ws.Cell(1, 7).Value = "Work Location";
                ws.Cell(1, 8).Value = "Department";
                ws.Cell(1, 9).Value = "Section";
                ws.Cell(1, 10).Value = "Job Title";

                ws.Column(1).Width = 20;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 20;
                ws.Column(4).Width = 20;
                ws.Column(5).Width = 20;
                ws.Column(6).Width = 30;
                ws.Column(7).Width = 30;
                ws.Column(8).Width = 20;
                ws.Column(9).Width = 20;
                ws.Column(10).Width = 20;

                int row = 2;
                foreach (var emp in employees)
                {
                    ws.Cell(row, 1).Value = emp.FirstName;
                    ws.Cell(row, 2).Value = emp.SecondName;
                    ws.Cell(row, 3).Value = emp.LastName;
                    ws.Cell(row, 4).Value = emp.Email;
                    ws.Cell(row, 5).Value = emp.Phone;
                    ws.Cell(row, 6).Value = emp.EmploymentDate.ToString("yyyy-MM-dd");
                    ws.Cell(row, 7).Value = emp.WorkLocation.ToString();
                    ws.Cell(row, 8).Value = emp.Department?.Name;
                    ws.Cell(row, 9).Value = emp.Section?.Name;
                    ws.Cell(row, 10).Value = emp.JobTitle?.Name;
                    row++;
                }

                using (var ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    return ms.ToArray();
                }
            }
        }





    }


    public class ExcelUploadResult
    {
        public int SuccessCount { get; set; }
        public List<string> Errors { get; set; }
    }

}

