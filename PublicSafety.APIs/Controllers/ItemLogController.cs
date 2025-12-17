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
    public class ItemLogController : Controller
    {
        [HttpGet]
        public JsonResult ByItem(Guid itemId)
        {
            if (itemId == Guid.Empty)
            {
                Response.StatusCode = 400;
                return Json(new ApiResponse<List<ItemLogDTO>>
                {
                    Success = false,
                    Message = "معرّف المادة غير صالح",
                    Data = new List<ItemLogDTO>()
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var logs = ItemLogService.GetItemLogsByItem(itemId);

                return Json(new ApiResponse<List<ItemLogDTO>>
                {
                    Success = true,
                    Message = null,
                    Data = logs ?? new List<ItemLogDTO>()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<List<ItemLogDTO>>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = new List<ItemLogDTO>()
                }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
