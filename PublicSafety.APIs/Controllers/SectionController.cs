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
    public class SectionController : Controller
    {

        [HttpGet]
        public JsonResult GetAllSections()
        {
            try
            {
                var sections = SectionService.GetAllSections();

                return Json(new ApiResponse<IEnumerable<SectionDTO>>
                {
                    Success = true,
                    Message = null,
                    Data = sections ?? new List<SectionDTO>()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<IEnumerable<SectionDTO>>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = new List<SectionDTO>()
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult GetSectionsByDepartmentId(Guid DepartmentId)
        {
            try
            {
                var sections = SectionService.GetSectionsByDepartmentId(DepartmentId);

                return Json(new ApiResponse<IEnumerable<SectionDTO>>
                {
                    Success = true,
                    Message = null,
                    Data = sections ?? new List<SectionDTO>()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<IEnumerable<SectionDTO>>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = new List<SectionDTO>()
                }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
