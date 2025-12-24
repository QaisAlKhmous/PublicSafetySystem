using PublicSafety.Domain.Entities;
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
    public class PlanningController : Controller
    {
        [HttpGet]
        public JsonResult Overview(int fromYear, int toYear)
        {
            if (fromYear <= 0 || toYear <= 0 || fromYear > toYear)
            {
                Response.StatusCode = 400;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "نطاق السنوات غير صالح",
                    Data = null
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var data = PlanningService.GetOverview(fromYear, toYear);

                return Json(new ApiResponse<List<PlanningOverview>>
                {
                    Success = true,
                    Message = null,
                    Data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<List<PlanningOverview>>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = null
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult PlannedItemsByYear(int year)
        {
            if (year <= 0)
            {
                Response.StatusCode = 400;
                return Json(new ApiResponse<List<PlanningItemDetails>>
                {
                    Success = false,
                    Message = "السنة غير صالحة",
                    Data = new List<PlanningItemDetails>()
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var data = PlanningService.GetPlannedItemDetails(year, year);

                return Json(new ApiResponse<List<PlanningItemDetails>>
                {
                    Success = true,
                    Message = null,
                    Data = data ?? new List<PlanningItemDetails>()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<List<PlanningItemDetails>>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = new List<PlanningItemDetails>()
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetYearEmployees(int year)
        {
            try
            {
                var yearEmployees = PlanningService.GetYearEmployees(year);

                if (yearEmployees == null)
                {
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Data = null,
                        Message = "لا يوجد موظفين هذه السنة"
                    },JsonRequestBehavior.AllowGet);

                }

                return Json(new ApiResponse<List<YearEmployeeSummaryDTO>>
                {
                    Success = true,
                    Data = yearEmployees,
                    Message = ""
                }, JsonRequestBehavior.AllowGet);

            }catch(Exception ex)
            {
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Data = null,
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
          



        }

    }
}
