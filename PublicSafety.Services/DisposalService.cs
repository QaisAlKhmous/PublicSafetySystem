using PublicSafety.Domain.Entities;
using PublicSafety.Repositories;
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
            using (var context = new AppDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var user = UserService.GetUserByUsername(newDisposal.CreatedBy);

                   
                    var disposal = new Disposal
                    {
                        DisposalId = Guid.NewGuid(),
                        ItemId = newDisposal.ItemId,
                        Quantity = newDisposal.Quantity,
                        DisposalDate = DateTime.Parse(newDisposal.DisposalDate),
                        DisposalFormPath = newDisposal.DisposalFormPath,
                        CreatedById = user.UserId,
                        CreatedDate = DateTime.Now
                    };

                    context.Disposals.Add(disposal);

                   
                    ItemService.DecreaseItemQuantity(newDisposal.ItemId, newDisposal.Quantity);

                   
                    var log = new ItemLog
                    {
                        ItemLogId = Guid.NewGuid(),
                        EmployeeId = null,               
                        ItemId = newDisposal.ItemId,
                        MatrixItemId = null,
                        IssuanceId = null,

                        ActionType = enItemActionType.Disposed,
                        Quantity = newDisposal.Quantity,
                        EntitlementYear = null,

                        Notes = $"إتلاف كمية {newDisposal.Quantity} من الصنف",

                        CreatedById = user.UserId,
                        CreatedDate = DateTime.Now
                    };

                    context.ItemLogs.Add(log);

                 
                    context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

    }
}
