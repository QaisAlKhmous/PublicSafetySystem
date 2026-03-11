using PublicSafety.Services;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PublicSafety.APIs.Controllers
{
    public class JobTitleController : Controller
    {
        [HttpGet]
        public JsonResult GetAllJobTitles()
        {
            try
            {
                var jobTitles = JobTitleService.GetAllJobTitles();

                return Json(new ApiResponse<IEnumerable<JobTitleDTO>>
                {
                    Success = true,
                    Message = null,
                    Data = jobTitles ?? new List<JobTitleDTO>()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<List<JobTitleDTO>>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = new List<JobTitleDTO>()
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetByDepartment(Guid departmentId)
        {
            try
            {
                var data = JobTitleService.GetJobTitlesByDepartment(departmentId);

                return Json(new ApiResponse<List<JobTitleDTO>>
                {
                    Success = true,
                    Data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<List<JobTitleDTO>>
                {
                    Success = false,
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetBySection(Guid SectionId)
        {
            try
            {
                var data = JobTitleService.GetJobTitlesBySection(SectionId);

                return Json(new ApiResponse<List<JobTitleDTO>>
                {
                    Success = true,
                    Data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<List<JobTitleDTO>>
                {
                    Success = false,
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AddJobTitle(AddJobTitleDTO model)
        {
            try
            {
                var result = JobTitleService.AddJobTitle(model);

                return Json(new
                {
                    Success = true,
                    Message = "تمت الإضافة بنجاح",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        public JsonResult GetAllJobTitlesHierarchy()
        {
            try
            {
                var data = JobTitleService.GetAllJobTitlesHierarchy();

                return Json(new
                {
                    Success = true,
                    Data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}
