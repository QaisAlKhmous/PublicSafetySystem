using DocumentFormat.OpenXml.Office2016.Excel;
using PublicSafety.Domain.Entities;
using PublicSafety.Repositories;
using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace PublicSafety.Services
{
    public class ChangeRequestService
    {
        public static void AddNewChangeRequest(ChangeRequestDTO changeRequest)
        {
            var newChangeRequest = new ChangeRequest()
            {
                RequestId = Guid.NewGuid(),
                OldValue = changeRequest.OldValue,
                NewValue = changeRequest.NewValue,
                AdminComment = changeRequest.AdminComment,
                ChangedById = UserService.GetUserByUsername(changeRequest.ChangedBy).UserId,
                RequestDate = DateTime.Now,
                EntityId = changeRequest.EntityId,
                Status = enRequestStatus.pending,
                EntityType = (enEntityType)Enum.Parse(typeof(enEntityType), changeRequest.EntityType),
               

            };
           ChangeRequestRepo.AddNewChangeRequest(newChangeRequest);
        }

        public static IEnumerable<ChangeRequestDTO> GetAllChangeRequests()
        {
            return ChangeRequestRepo.GetAllChangeRequests().Select(cr => new ChangeRequestDTO
            {
                RequestId = cr.RequestId,
                OldValue = cr.OldValue,
                NewValue = cr.NewValue,
                AdminComment = cr.AdminComment,
                ChangedBy = cr.ChangedBy.Username,
                EntityId = cr.EntityId,
                EntityType = cr.EntityType.ToString(),
                RequestDate = cr.RequestDate.ToString("yyyy-MM-dd"),
                Status = (int)cr.Status,
                ApprovedDate = cr.ApprovedDate?.ToString("yyyy-MM-dd")
            });
        }

        public static ChangeRequestDTO GetChangeRequestById(Guid ChangeRequestId)
        {
            var changeRequest = ChangeRequestRepo.GetChangeRequestById(ChangeRequestId);

            return new ChangeRequestDTO()
            {
                RequestId = changeRequest.RequestId,
                OldValue = changeRequest.OldValue,
                NewValue = changeRequest.NewValue,
                AdminComment = changeRequest.AdminComment,
                ChangedBy = changeRequest.ChangedBy.Username,
                EntityId = changeRequest.EntityId,
                EntityType = changeRequest.EntityType.ToString(),
                RequestDate = changeRequest.RequestDate.ToString(),
                Status = (int)changeRequest.Status
            };
        }

        private static void _AcceptAddEntity( ChangeRequestDTO changeRequest)
        {
            switch((enEntityType)Enum.Parse(typeof(enEntityType), changeRequest.EntityType))
            {
                case enEntityType.Employee:
                    EmployeeService.AddNewEmployee(JsonSerializer.Deserialize<AddEmployeeDTO>(changeRequest.NewValue)); 
                    break;

                case enEntityType.Item:
                    ItemService.AddItem(JsonSerializer.Deserialize<ItemsDTO>(changeRequest.NewValue));
                    break;
                case enEntityType.Matrix:
                    MatrixService.CreateNewMatrixVersion
(JsonSerializer.Deserialize<MatrixDTO>(changeRequest.NewValue).CategoryId);
                    break;
                case enEntityType.Issuance:
                    var issuance = JsonSerializer.Deserialize<AddIssuanceDTO>(changeRequest.NewValue);
                    if ((enIssuanceType)Enum.Parse(typeof(enIssuanceType), issuance.Type) == enIssuanceType.Entitled)
                        IssuanceService.AddNewEntitledIssuance(issuance);
                    else
                        IssuanceService.AddNewIssuance(issuance);
                    break;
            }
        }

        private static void _AcceptUpdateEntity(ChangeRequestDTO changeRequest)
        {
            switch ((enEntityType)Enum.Parse(typeof(enEntityType), changeRequest.EntityType))
            {
                case enEntityType.Employee:
                    AddEmployeeDTO employee = JsonSerializer.Deserialize<AddEmployeeDTO>(changeRequest.NewValue);
                    employee.EmployeeId = changeRequest.EntityId;
                    EmployeeService.UpdateEmployee(employee);
                    break;

                case enEntityType.Item:
                    var item = JsonSerializer.Deserialize<itemRequestDTO>(changeRequest.OldValue);

                   if(item.IsIncrease)
                        ItemService.IncreaseItemQuantity(changeRequest.EntityId, item.Quantity,changeRequest.ChangedBy);
                   else ItemService.DecreaseItemQuantity(changeRequest.EntityId, item.Quantity);
                        break;
                case enEntityType.Matrix:
                    MatrixService.CreateNewMatrixVersion(JsonSerializer.Deserialize<MatrixDTO>(changeRequest.NewValue).CategoryId);
                    break;
            }
        }

        public static void AcceptChangeRequest(Guid ChangeRequestId, string ApprovedBy)
        {
            var changeRequest = GetChangeRequestById(ChangeRequestId);
            changeRequest.ApprovedBy = ApprovedBy;
            changeRequest.ApprovedDate = DateTime.Now.ToString();
            changeRequest.Status = 1;
            // add entity request
            if (changeRequest.OldValue == null)
            {
                _AcceptAddEntity(changeRequest);
            }
            // edit entity request
            else
            {
                _AcceptUpdateEntity(changeRequest);
            }

            var updatedChangeRequest = new ChangeRequest()
            {
                AdminComment = changeRequest.AdminComment,
                ApprovedById = UserService.GetUserByUsername(changeRequest.ApprovedBy).UserId,
                ApprovedDate = DateTime.Parse(changeRequest.ApprovedDate),
                EntityId = changeRequest.EntityId,
                Status = (enRequestStatus)changeRequest.Status,
                RequestId = changeRequest.RequestId
            };

            ChangeRequestRepo.UpdateChangeRequest(updatedChangeRequest);
        }

        public static void RejectChangeRequest(Guid ChangeRequestId, string ApprovedBy)
        {
            var changeRequest = GetChangeRequestById(ChangeRequestId);
            changeRequest.ApprovedBy = ApprovedBy;
            changeRequest.ApprovedDate = DateTime.Now.ToString();
            changeRequest.Status = 2;

            var updatedChangeRequest = new ChangeRequest()
            {
                AdminComment = changeRequest.AdminComment,
                ApprovedById = UserService.GetUserByUsername(changeRequest.ApprovedBy).UserId,
                ApprovedDate = DateTime.Parse(changeRequest.ApprovedDate),
                EntityId = changeRequest.EntityId,
                Status = (enRequestStatus)changeRequest.Status,
                RequestId = changeRequest.RequestId
            };

            ChangeRequestRepo.UpdateChangeRequest(updatedChangeRequest);
        }

       

        public static List<DifferenceResult> GetDifferences(Guid ChangeRequestId)
        {
            var changeRequest = GetChangeRequestById(ChangeRequestId);

            var oldData = string.IsNullOrEmpty(changeRequest.OldValue)
                ? new Dictionary<string, JsonElement>()
                : JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(changeRequest.OldValue);

            var newData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(changeRequest.NewValue);

            var differences = new List<DifferenceResult>();

            foreach (var key in newData.Keys)
            {
                oldData.TryGetValue(key, out JsonElement oldValueElement);
                var newValueElement = newData[key];

                string oldValue = oldValueElement.ValueKind == JsonValueKind.Undefined
                    ? null
                    : oldValueElement.ToString();

                string newValue = newValueElement.ToString();

                // If it's a new record
                if (oldValue == null)
                {
                    differences.Add(new DifferenceResult
                    {
                        Field = key,
                        OldValue = null,
                        NewValue = newValue
                    });
                    continue;
                }

                // If value changed
                if (oldValue != newValue)
                {
                    differences.Add(new DifferenceResult
                    {
                        Field = key,
                        OldValue = oldValue,
                        NewValue = newValue
                    });
                }
            }

            return differences;
        }

       
    }
    public class DifferenceResult
    {
        public string Field { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}
