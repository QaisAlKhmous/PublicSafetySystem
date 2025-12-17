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
    public class IssuanceController : Controller
    {
        [HttpPost]
        public JsonResult AddNewIssuance(AddIssuanceDTO issuance)
        {
            // 🔹 Basic input validation
            if (issuance == null ||
                issuance.ItemId == Guid.Empty ||
                issuance.Quantity <= 0)
            {
                Response.StatusCode = 400;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "بيانات الإصدار غير صالحة",
                    Data = null
                });
            }

            var item = ItemService.GetItemById(issuance.ItemId);

            if (item == null)
            {
                Response.StatusCode = 404;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "المادة غير موجودة",
                    Data = null
                });
            }

            // 🔹 Business validation
            if (item.Quantity < issuance.Quantity)
            {
                Response.StatusCode = 409; // Conflict
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "الكمية المطلوبة أكبر من المتوفرة",
                    Data = null
                });
            }

            try
            {
                IssuanceService.AddNewIssuance(issuance);

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = "تم الإصدار بنجاح",
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

        [HttpPost]
        public JsonResult AddNewEntitledIssuance(AddIssuanceDTO issuance)
        {
            if (issuance == null ||
                issuance.ItemId == Guid.Empty ||
                issuance.Quantity <= 0)
            {
                Response.StatusCode = 400;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "بيانات الإصدار غير صالحة",
                    Data = null
                });
            }

            var item = ItemService.GetItemById(issuance.ItemId);

            if (item == null)
            {
                Response.StatusCode = 404;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "المادة غير موجودة",
                    Data = null
                });
            }

            if (item.Quantity < issuance.Quantity)
            {
                Response.StatusCode = 409; // Conflict
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "الكمية المطلوبة أكبر من المتوفرة",
                    Data = null
                });
            }

            try
            {
                IssuanceService.AddNewEntitledIssuance(issuance);

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = "تم الإصدار بنجاح",
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

        [HttpGet]
        public JsonResult GetIssuancesByEmployeeId(Guid employeeId)
        {
            if (employeeId == Guid.Empty)
            {
                Response.StatusCode = 400;
                return Json(new ApiResponse<List<IssuanceDTO>>
                {
                    Success = false,
                    Message = "معرّف الموظف غير صالح",
                    Data = new List<IssuanceDTO>()
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var issuances = IssuanceService.GetIssuancesByEmployeeId(employeeId);

                return Json(new ApiResponse<IEnumerable<IssuanceDTO>>
                {
                    Success = true,
                    Message = null,
                    Data = issuances ?? new List<IssuanceDTO>()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<IEnumerable<IssuanceDTO>>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = new List<IssuanceDTO>()
                }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
