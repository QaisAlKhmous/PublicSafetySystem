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
            IssuanceRepo.AddIssuance(newIssuance);
        }
    }
}
