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
        public JsonResult AddNewIssuance(AddIssuanceDTO issuance)
        {
            var item = ItemService.GetItemById(issuance.ItemId);

            if (item.Quantity < issuance.Quantity)
            {
                return Json(new
                {
                    success = false,
                    message = "الكمية المطلوبة أكبر من المتوفرة!"
                });
            }

            IssuanceService.AddNewIssuance(issuance);

            return Json(new
            {
                success = true,
                message = "تم الاصدار بنجاح"
            });
        }
        [HttpGet]
        public JsonResult GetIssuancesByEmployeeId(Guid EmployeeId)
        {
            return Json(IssuanceService.GetIssuancesByEmployeeId(EmployeeId),JsonRequestBehavior.AllowGet);
        }
    }
}
