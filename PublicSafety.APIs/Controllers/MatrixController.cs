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
    public class MatrixController : Controller
    {
        //[HttpGet]
        //public JsonResult GetAllMatrices()
        //{
        //    var items = MatrixService.GetAllMatrices();

        //    return Json(items, JsonRequestBehavior.AllowGet);


        //}

        [HttpGet]
        public JsonResult GetMatrixByCategory(Guid CategoryId)
        {
            var matrix =  MatrixService.GetMatrixByCategory(CategoryId);

            return Json(matrix,JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetItemsByMatrix(Guid MatrxiId)
        {
            var items = MatrixService.GetItemsByMatrix(MatrxiId);

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetItemsByCategory(Guid CategoryId)
        {
            var items = MatrixService.GetItemsByCategory(CategoryId);

            if(items == null)
                return Json(new {success =  false,error = 1},JsonRequestBehavior.AllowGet);

     

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IsMatrixExistsByCategoryId(Guid CategoryId)
        {
            return Json(MatrixService.IsMatrixExistsForCategory(CategoryId),JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddNewMatrix(Guid CategoryId)
        {

            var id = MatrixService.AddNewMatrix(CategoryId);

            return Json(id);
        }

        [HttpPost]
        public JsonResult AddItemInMatrix(MatrixItemDTO MatrixItem)
        {


           var id = MatrixService.AddNewMatrixItem(MatrixItem);

            return Json(id);
        }

        [HttpPost]
        public JsonResult DeleteMatrixItem(Guid MatrixItemId)
        {


            MatrixService.DeleteMatrixItem(MatrixItemId);

            return Json(new {success = true});
        }

        [HttpPost]
        public JsonResult UpdateMatrixItem(UpdateMatrixItemDTO MatrixItem)
        {


            MatrixService.UpdateMatrixItem(MatrixItem);

            return Json(new { success = true });
        }


    }
}
