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
        public JsonResult GetMatrixByCategory(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                Response.StatusCode = 400;
                return Json(new ApiResponse<List<MatrixDTO>>
                {
                    Success = false,
                    Message = "معرّف الفئة غير صالح",
                    Data = new List<MatrixDTO>()
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var matrix = MatrixService.GetMatrixByCategory(categoryId);
                
                if(matrix == null)
                {
                    Response.StatusCode = 404;
                    return Json(new ApiResponse<IEnumerable<MatrixDTO>>
                    {
                        Success = false,
                        Message = null,
                        Data = new List<MatrixDTO>()
                    }, JsonRequestBehavior.AllowGet);
                }
                   


                return Json(new ApiResponse<MatrixDTO>
                {
                    Success = true,
                    Message = null,
                    Data = matrix
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<MatrixDTO>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data =new MatrixDTO()
                }, JsonRequestBehavior.AllowGet);
            }
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
            var id = MatrixService.CreateNewMatrixVersion(CategoryId);

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
