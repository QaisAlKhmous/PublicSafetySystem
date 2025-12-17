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
    public class DisposalController : Controller
    {

        [HttpPost]
        public JsonResult AddDisposal(DisposalDTO disposal)
        {
            if (disposal == null)
            {
                Response.StatusCode = 400;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "بيانات الإتلاف غير صالحة",
                    Data = null
                });
            }

            try
            {
                DisposalService.AddNewDisposal(disposal);

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = "تمت الإضافة بنجاح",
                    Data = null
                });
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = null
                });
            }
        }


    }
}
