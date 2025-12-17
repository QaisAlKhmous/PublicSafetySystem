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
    public class ItemController : Controller
    {

        [HttpGet]

        public JsonResult GetItems()
        {


            try
            {
                var items = ItemService.GetAllItems();


                if (items == null)
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "مشكلة في السيرفر",
                        Data = null
                    }, JsonRequestBehavior.AllowGet);


                return Json(new ApiResponse<IEnumerable<ItemsDTO>>
                {
                    Success = true,
                    Message = "",
                    Data = items
                }, JsonRequestBehavior.AllowGet);
            }

             catch(Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<AddEmployeeDTO>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = null
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult GetItemById(Guid id)
        {
            if (id == Guid.Empty)
            {
                Response.StatusCode = 400;
                return Json(new ApiResponse<ItemsDTO>
                {
                    Success = false,
                    Message = "معرّف العنصر غير صالح",
                    Data = null
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var item = ItemService.GetItemById(id);

                if (item == null)
                {
                    Response.StatusCode = 404;
                    return Json(new ApiResponse<ItemsDTO>
                    {
                        Success = false,
                        Message = "العنصر غير موجود",
                        Data = null
                    }, JsonRequestBehavior.AllowGet);
                }

                // ✅ SUCCESS
                return Json(new ApiResponse<ItemsDTO>
                {
                    Success = true,
                    Message = null,
                    Data = item
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<ItemsDTO>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = null
                }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public JsonResult AddItem(ItemsDTO item)
        {
            if (item == null)
            {
                Response.StatusCode = 400;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "البيانات غير صالحة",
                    Data = null
                });
            }

            if (ItemService.IsItemExistsByName(item.Name))
            {
                Response.StatusCode = 409; // Conflict
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "العنصر موجود مسبقاً",
                    Data = null
                });
            }

            try
            {
                ItemService.AddItem(item);

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


        [HttpPost]
        public JsonResult DeleteItem(Guid id)
        {
            ItemService.DeleteItem(id);

            return Json("Deleted Successfully!");


        }

        [HttpPost]
        public JsonResult IncreaseQuantity(Guid id, int quantity, string createdBy)
        {
            if (id == Guid.Empty || quantity <= 0)
            {
                Response.StatusCode = 400;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "بيانات غير صالحة",
                    Data = null
                });
            }

            try
            {
                ItemService.IncreaseItemQuantity(id, quantity, createdBy);

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = "تم تحديث الكمية بنجاح",
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
        public JsonResult DecreaseQuantity(Guid id, int quantity)
        {
            if (id == Guid.Empty || quantity <= 0)
            {
                Response.StatusCode = 400;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "بيانات غير صالحة",
                    Data = null
                });
            }

            try
            {
                ItemService.DecreaseItemQuantity(id, quantity);

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = "تم تحديث الكمية بنجاح",
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
        public JsonResult GetNumberOfAllItems()
        {
            try
            {
                var count = ItemService.GetNumberOfAllItems();

                // Validation: service returned invalid value
                if (count < 0)
                {
                    Response.StatusCode = 500;
                    return Json(new ApiResponse<int>
                    {
                        Success = false,
                        Message = "قيمة غير صالحة من السيرفر",
                        Data = 0
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new ApiResponse<int>
                {
                    Success = true,
                    Message = "",
                    Data = count
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log error here (very important in real apps)
                Response.StatusCode = 500;

                return Json(new ApiResponse<int>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult IsQuantityEnough(Guid Id,int Quantity)
        {
            return Json(ItemService.IsQuantityEnough(Id, Quantity),JsonRequestBehavior.AllowGet);
        }
    }
}
