using PublicSafety.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PublicSafety.APIs.Controllers
{
    public class UploadController: Controller
    {
        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase uploadFile)
        {
            string tempId = "file" + "_" + Guid.NewGuid().ToString().Replace("-", "");
            HttpPostedFileBase File = Request.Files["file"];
            if (File != null)
            {
                if (File.ContentLength > 0)
                {
                    string ext = Path.GetExtension(File.FileName);
                    tempId = tempId + ext;

                    string uploadDir = HttpContext.Server.MapPath("~/Uploads/");

                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string filePath = Path.Combine(uploadDir, tempId);

                    File.SaveAs(filePath);
                }
            }

            return Json(new { tempId = tempId });
        }


        [HttpGet]
        public ActionResult GetUploadedFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return HttpNotFound();

            string filePath = Server.MapPath("~" + fileName);

            if (!System.IO.File.Exists(filePath))
                return HttpNotFound();

            // Determine content type
            string contentType = MimeMapping.GetMimeMapping(filePath);

            return File(filePath, contentType, fileName);
        }


        [HttpPost]
        public JsonResult UploadEmployeesExcel(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return Json(new { SuccessCount = 0, Errors = new List<string> { "No file uploaded" } });

            var result = ExcelService.AddEmployeesFromExcel(file.InputStream);

            return Json(result);
        }
        public FileResult DownloadEmployeeTemplate()
        {
            var fileBytes = ExcelService.GenerateEmployeeTemplate();
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "EmployeeUploadTemplate.xlsx");
        }


        [HttpGet]
        public ActionResult DownloadAllEmployees()
        {
            var fileContents = ExcelService.ExportAllEmployeesToExcel();
            var fileName = $"Employees_{DateTime.Now:yyyyMMddHHmm}.xlsx";

            return File(fileContents,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
        }

    }
}
