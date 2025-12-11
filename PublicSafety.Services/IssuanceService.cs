using PublicSafety.Domain.Entities;
using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public class IssuanceService
    {
        public static void AddNewIssuance(AddIssuanceDTO issuance)
        {

            

            var newIssuance = new Issuance()
            {
                IssuanceId = Guid.NewGuid(),
                ItemId = issuance.ItemId,
                Quantity = issuance.Quantity,
                IssuanceDate = DateTime.Now,
                ExceptionFormPath = issuance.ExceptionFormPath,
                SignedReceiptPath = issuance.SignedReceiptPath,
                CreatedById = UserService.GetUserByUsername(issuance.CreatedBy).UserId,
                Type = (enIssuanceType)Enum.Parse(typeof(enIssuanceType), issuance.Type),
                ExceptionReason = issuance.ExceptionReason != null ? issuance.ExceptionReason : "",
                CreatedDate = DateTime.Now,
                EmployeeId = issuance.EmployeeId
            };

            if((enIssuanceType)Enum.Parse(typeof(enIssuanceType), issuance.Type) == enIssuanceType.Entitled)
            {
                newIssuance.IssuanceDate = DateTime.Parse(issuance.IssuanceDate);
            }
            IssuanceRepo.AddIssuance(newIssuance);
            ItemService.DecreaseItemQuantity(issuance.ItemId, issuance.Quantity);
        }

        public static IEnumerable<IssuanceDTO> GetIssuancesByEmployeeId(Guid EmployeeId)
        {
            var issuances = IssuanceRepo.GetIssuancesByEmployeeId(EmployeeId);

            return issuances.Select(i => new IssuanceDTO()
            {
                Item = i.Item.Name,
                CreatedBy = i.CreatedBy.Username,
                Quantity = i.Quantity,
                ExceptionReason = i.ExceptionReason,
                ExceptionFormPath = i.ExceptionFormPath,
                IssuanceDate = i.IssuanceDate.ToString("yyyy-MM-dd"),
                SignedReceiptPath = i.SignedReceiptPath,
                Type = i.Type.ToString()
            });
        }
    }
}
