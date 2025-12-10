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

            return Json(Employees, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult AddNewEmployee(AddEmployeeDTO Employee)
        {
          Guid? id =  EmployeeService.AddNewEmployee(Employee);

            return Json(new { id = id }, "Added Succesfully!");
        }

        [HttpPost]
        public JsonResult ResignEmployee(Guid Id)
        {
            EmployeeService.ResignEmployee(Id);

            return Json("Resigned Succesfully!");
        }

        [HttpGet]
        public JsonResult GetEmployee(Guid Id)
        {
          var employee = EmployeeService.GetEmployeeById(Id);

            return Json(employee, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateEmployee(AddEmployeeDTO Employee)
        {
            EmployeeService.UpdateEmployee(Employee);

            return Json("updated successfully");
        }

        [HttpGet]
        public JsonResult GetNumberOfActiveEmployees()
        {
          int number =  EmployeeService.GetNumberOfActiveEmployees();

            return Json(number,JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetNumberOfInactiveEmployees()
        {
            int number = EmployeeService.GetNumberOfInactiveEmployees();

            return Json(number, JsonRequestBehavior.AllowGet);
        }
    }
}
