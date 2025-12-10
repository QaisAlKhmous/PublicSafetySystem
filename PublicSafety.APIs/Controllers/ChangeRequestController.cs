using PublicSafety.Domain.Entities;
using PublicSafety.Services;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;

namespace PublicSafety.APIs.Controllers
{
    public class ChangeRequestController : Controller
    {
        [HttpPost]
        public JsonResult AddNewChangeRequest(ChangeRequestDTO ChangeRequest)
        {
            ChangeRequestService.AddNewChangeRequest(ChangeRequest);

            return Json(new { success = true });
        }
        [HttpGet]
        public JsonResult GetAllChangeRequests()
        {
            return Json(ChangeRequestService.GetAllChangeRequests(),JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AcceptChangeRequest(Guid ChangeRequestId,string ApprovedBy)
        {
            ChangeRequestService.AcceptChangeRequest(ChangeRequestId, ApprovedBy);

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult RejectChangeRequest(Guid ChangeRequestId, string ApprovedBy)
        {
            ChangeRequestService.RejectChangeRequest(ChangeRequestId, ApprovedBy);

            return Json(new { success = true });
        }
        
        

        [HttpPost]
        public JsonResult GetDifferences(Guid ChangeRequestId,Guid EntityId)
        {
            var changeRequest = ChangeRequestService.GetChangeRequestById(ChangeRequestId);

            if(changeRequest.OldValue == null)
            {
                switch ((enEntityType)Enum.Parse(typeof(enEntityType), changeRequest.EntityType))
                {
                    case enEntityType.Employee:
                        AddEmployeeDTO employee = JsonSerializer.Deserialize<AddEmployeeDTO>(changeRequest.NewValue);

                        EmployeeDTO emp = new EmployeeDTO()
                        {
                            Category = CategoryService.GetCategoryByJobTitleId(employee.JobTitleId).Category,
                            HealthInsuranceFile = employee.HealthInsuranceFile,
                            Department = DepartmentService.GetDepartmentById(employee.DepartmentId).Name,
                            Section = SectionService.GetSectionById(employee.SectionId).Name,
                            Email = employee.Email,
                            EmploymentDate = employee.EmploymentDate,
                            FullName = employee.FirstName + " " + employee.SecondName + " " + employee.LastName,
                            JobTitle = CategoryService.GetCategoryByJobTitleId(employee.JobTitleId).JobTitle,
                            Phone = employee.Phone,
                            WorkLocation = employee.WorkLocation
                        };
                        return Json(new {entity = emp, type="employee",IsAdd = true }, JsonRequestBehavior.AllowGet);
                       

                    case enEntityType.Item:
                        var item = JsonSerializer.Deserialize<ItemsDTO>(changeRequest.NewValue);
                        
                        return Json(new { entity = item, type = "item", IsAdd = true });
                       
                       
                    case enEntityType.Issuance:
                        var issuance = JsonSerializer.Deserialize<AddIssuanceDTO>(changeRequest.NewValue);
                        string EmployeeName = EmployeeService.GetEmployeeById(issuance.EmployeeId).FullName;
                        string itemIssued = ItemService.GetItemById(issuance.ItemId).Name;
                       return Json(new { entity = issuance, employee = EmployeeName,item = itemIssued, type = "issuance", IsAdd = true }, JsonRequestBehavior.AllowGet);


                }
            }
            else
            {
                switch ((enEntityType)Enum.Parse(typeof(enEntityType), changeRequest.EntityType))
                {
                    case enEntityType.Employee:
                        AddEmployeeDTO oldEmployee = JsonSerializer.Deserialize<AddEmployeeDTO>(changeRequest.OldValue);
                        AddEmployeeDTO newEmployee = JsonSerializer.Deserialize<AddEmployeeDTO>(changeRequest.NewValue);

                        EmployeeDTO oldEmp = new EmployeeDTO()
                        {
                            Category = CategoryService.GetCategoryByJobTitleId(oldEmployee.JobTitleId).Category,
                            HealthInsuranceFile = oldEmployee.HealthInsuranceFile,
                            Department = DepartmentService.GetDepartmentById(oldEmployee.DepartmentId).Name,
                            Section = SectionService.GetSectionById(oldEmployee.SectionId).Name,
                            Email = oldEmployee.Email,
                            EmploymentDate = oldEmployee.EmploymentDate,
                            FullName = oldEmployee.FirstName + " " + oldEmployee.SecondName + " " + oldEmployee.LastName,
                            JobTitle = CategoryService.GetCategoryByJobTitleId(oldEmployee.JobTitleId).JobTitle,
                            Phone = oldEmployee.Phone,
                            WorkLocation = oldEmployee.WorkLocation
                        };

                        EmployeeDTO newEmp = new EmployeeDTO()
                        {
                            Category = CategoryService.GetCategoryByJobTitleId(newEmployee.JobTitleId).Category,
                            HealthInsuranceFile = newEmployee.HealthInsuranceFile,
                            Department = DepartmentService.GetDepartmentById(newEmployee.DepartmentId).Name,
                            Section = SectionService.GetSectionById(newEmployee.SectionId).Name,
                            Email = newEmployee.Email,
                            EmploymentDate = newEmployee.EmploymentDate,
                            FullName = newEmployee.FirstName + " " + newEmployee.SecondName + " " + newEmployee.LastName,
                            JobTitle = CategoryService.GetCategoryByJobTitleId(newEmployee.JobTitleId).JobTitle,
                            Phone = newEmployee.Phone,
                            WorkLocation = newEmployee.WorkLocation
                        };
                        return Json(new { oldEntity = oldEmp, newEntity = newEmp, type = "employee", IsAdd = false });


                    case enEntityType.Item:
                        var itemReq = JsonSerializer.Deserialize<itemRequestDTO>(changeRequest.OldValue);
                        var item = ItemService.GetItemById(changeRequest.EntityId);
                        return Json(new { entity = item, type = "item", IsAdd = false, itemReq = itemReq }, JsonRequestBehavior.AllowGet);

                    
                   

                }
            }
                return Json(ChangeRequestService.GetDifferences(ChangeRequestId), JsonRequestBehavior.AllowGet);
        }
    }
}
