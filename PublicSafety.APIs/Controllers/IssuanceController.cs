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


        [HttpPost]
        public ActionResult IssueMatrixForCategory(Guid categoryId, int year,Guid UserId,string SignedReceiptPath)
        {
            try
            {
                IssuanceService.IssueMatrixForCategory(categoryId, year,UserId, SignedReceiptPath);
                Response.StatusCode = 200;
                return Json(new ApiResponse<object>
                {

                    Success = true,
                    Message = "تم الإصدار بنجاح",
                    Data = null
                });

            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
           
        }
        [HttpPost]
        public ActionResult IssueEmployeeEntitlementsForYear(IssueEmployeeYearDTO model)
        {
            try
            {
             
                if (model == null)
                    return new HttpStatusCodeResult(400, "البيانات غير صحيحة");

                if (model.EmployeeId == Guid.Empty)
                    return new HttpStatusCodeResult(400, "الموظف غير محدد");

                if (model.Year <= 0)
                    return new HttpStatusCodeResult(400, "سنة الاستحقاق غير صحيحة");

                if (string.IsNullOrWhiteSpace(model.SignedReceiptPath))
                    return new HttpStatusCodeResult(400, "يرجى رفع نموذج التوقيع");

              
                IssuanceService.IssueEmployeeEntitlementsForYear(
                    model
                );

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = "تم صرف استحقاقات سنة " + model.Year + " بنجاح",
                    Data = null
                });
            }
            catch (Exception ex)
            {
              
                Response.StatusCode = 500;

                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }



        [HttpPost]
        public ActionResult UploadSignedReceipt(
    Guid employeeId,
    int entitlementYear,
    string receiptPath)
        {
            try
            {
                IssuanceService.AttachSignedReceipt(
                    employeeId,
                    entitlementYear,
                    receiptPath
                );

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = "تم تحديث إيصال الاستلام بنجاح",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

    }
}
