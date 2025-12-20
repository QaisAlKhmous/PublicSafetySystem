using PublicSafety.Domain.Entities;
using PublicSafety.Services;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PublicSafety.APIs.Controllers
{
    public class EmployeeController : Controller
    {
        [HttpGet]
        public JsonResult GetAllEmployees()
        {
            var Employees = EmployeeService.GetAllEmployees();

            if (Employees == null || Employees.Count() == 0)
                return Json(new List<EmployeeDTO>(),JsonRequestBehavior.AllowGet);

            return Json(Employees, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult AddNewEmployee(AddEmployeeDTO Employee)
        {
            try
            {
                Guid id = EmployeeService.AddNewEmployee(Employee);

                Response.StatusCode = 200;
                return Json(new ApiResponse<Guid>
                {
                    Success = false,
                    Message = "Unexpected server error",
                    Data = id
                });

            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Unexpected server error"
                });
            }
        
        }

        [HttpPost]
        public JsonResult ResignEmployee(Guid Id)
        {

            if (Id == Guid.Empty)
            {
                Response.StatusCode = 400;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "معرّف الموظف غير صالح",
                    Data = null
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                EmployeeService.ResignEmployee(Id);
                Response.StatusCode = 200;
                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = "استقال الموظف بنجاح"
                });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Unexpected server error"
                });
            }
           
        }

        [HttpGet]
        public JsonResult GetEmployee(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                Response.StatusCode = 400;
                return Json(new ApiResponse<AddEmployeeDTO>
                {
                    Success = false,
                    Message = "معرّف الموظف غير صالح",
                    Data = null
                }, JsonRequestBehavior.AllowGet);
            }


            try
            {
                var employee = EmployeeService.GetEmployeeById(Id);

                if (employee == null)
                {
                    Response.StatusCode = 404;
                    return Json(new ApiResponse<AddEmployeeDTO>
                    {
                        Success = false,
                        Message = "الموظف غير موجود",
                        Data = null
                    }, JsonRequestBehavior.AllowGet);
                }
                Response.StatusCode = 200;
                return Json(new ApiResponse<AddEmployeeDTO>
                {
                    Success = true,
                    Message = "",
                    Data = employee
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

        [HttpPost]
        public JsonResult UpdateEmployee(AddEmployeeDTO Employee)
        {
            try
            {
                if (Employee == null)
                {
                    Response.StatusCode = 400;
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "الموظف غير موجود",
                        Data = null
                    }, JsonRequestBehavior.AllowGet);
                }

               if(EmployeeService.UpdateEmployee(Employee))
                {
                    Response.StatusCode = 200;
                    return Json(new ApiResponse<object>
                    {
                        Success = true,
                        Message = "تم تعديل الموظف بنجاح",
                        Data = null
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Response.StatusCode = 500;
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "حدث خطأ",
                        Data = null
                    }, JsonRequestBehavior.AllowGet);
                }

               

            }
            catch(Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = null
                }, JsonRequestBehavior.AllowGet);
            }
           
        }

        [HttpGet]
        public JsonResult GetNumberOfActiveEmployees()
        {
            try
            {
                int number = EmployeeService.GetNumberOfActiveEmployees();

                return Json(new ApiResponse<int>
                {
                    Success = true,
                    Message = null,
                    Data = number
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<int>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetNumberOfInactiveEmployees()
        {
            try
            {
                int number = EmployeeService.GetNumberOfInactiveEmployees();

                return Json(new ApiResponse<int>
                {
                    Success = true,
                    Message = null,
                    Data = number
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                Response.StatusCode = 500;
                return Json(new ApiResponse<int>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult GetEmployeeEntitlements(Guid EmployeeId)
        {
            var entitlements = EntitlementService.GetEntitlementsByEmployeeId(EmployeeId);

            if(entitlements == null)
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = null
                }, JsonRequestBehavior.AllowGet);

            return Json(new ApiResponse<IEnumerable<Entitlement>>
            {
                Success = true,
                Message = "",
                Data = entitlements
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEmployeesByCategory()
        {
            var ebc = EmployeeService.GetEmployeesByCategoriesCount();

            if (ebc == null)
                return Json(new ApiResponse<object>
                {
                    Success = false,
                    Message = "مشكلة في السيرفر",
                    Data = null
                }, JsonRequestBehavior.AllowGet);

            return Json(new ApiResponse<IEnumerable<EmployeesByCategory>>
            {
                Success = true,
                Message = "",
                Data = ebc
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ActivateEmployee(ActivateEmployeeDTO model)
        {
            try
            {
              
                if (model == null)
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "البيانات غير صحيحة"
                    });

                if (model.EmployeeId == Guid.Empty)
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "الموظف غير محدد"
                    });

                if (model.JobTitleId == Guid.Empty)
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "المسمى الوظيفي مطلوب"
                    });

                if (model.DepartmentId == Guid.Empty)
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "القسم مطلوب"
                    });

                if (model.SectionId == Guid.Empty)
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "الشعبة مطلوبة"
                    });

                if (DateTime.Parse(model.ActivationDate) == DateTime.MinValue)
                    return Json(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "تاريخ إعادة التفعيل غير صحيح"
                    });

              
                EmployeeService.ActivateEmployee(model);

                return Json(new ApiResponse<object>
                {
                    Success = true,
                    Message = "تمت إعادة تفعيل الموظف بنجاح",
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

    }
}
