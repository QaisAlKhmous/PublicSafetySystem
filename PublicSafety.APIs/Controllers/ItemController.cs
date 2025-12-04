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
            var items = ItemService.GetAllItems();

            return Json(items, JsonRequestBehavior.AllowGet);





        }

        [HttpPost]
        public JsonResult AddItem(ItemsDTO item)
        {
            ItemService.AddItem(item);

            return Json("Added Successfully!");


        }

        [HttpPost]
        public JsonResult DeleteItem(Guid id)
        {
            ItemService.DeleteItem(id);

            return Json("Deleted Successfully!");


        }

        [HttpPost]
        public JsonResult IncreaseQuantity(Guid id,int quantity)
        {
            ItemService.IncreaseItemQuantity(id, quantity);

            return Json("Updated Successfully!");


        }

        [HttpPost]
        public JsonResult DecreaseQuantity(Guid id, int quantity)
        {
            ItemService.DecreaseItemQuantity(id, quantity);

            return Json("Updated Successfully!");


        }
    }
}
