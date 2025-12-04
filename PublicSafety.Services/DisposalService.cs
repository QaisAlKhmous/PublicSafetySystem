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
    public class DisposalService
    {
        public static void AddNewDisposal(DisposalDTO newDisposal)
        {

            var disposal = new Disposal() { DisposalId = Guid.NewGuid(),ItemId = newDisposal.ItemId,Quantity = newDisposal.Quantity
            ,DisposalDate =DateTime.Parse(newDisposal.DisposalDate),CreatedDate = DateTime.Now ,DisposalFormPath = newDisposal.DisposalFormPath,
                CreatedById = UserService.GetUserByUsername(newDisposal.CreatedBy).UserId};
            DisposalRepo.AddNewDisposal(disposal);

            ItemService.DecreaseItemQuantity(newDisposal.ItemId, newDisposal.Quantity);
        }
    }
}
