using AdminDashboard.Services;
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
    public class EmailController : Controller
    {

        [HttpPost]
        public JsonResult SendIssueNotificationToManager(Guid UserId)
        {
            try
            {
                EmailService.SendIssueNotificationToManager(UserId);

                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "تم ارسال الاشعار عبر الايميل",
                    Data = new List<SectionDTO>()
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = new List<SectionDTO>()
                }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
