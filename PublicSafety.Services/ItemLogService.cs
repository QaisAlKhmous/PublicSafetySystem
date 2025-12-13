using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public static class ItemLogService
    {
        public static List<ItemLogDTO> GetItemLogsByItem(Guid itemId)
        {
            var logs = ItemLogRepo.GetItemLogsByItemId(itemId);

            return logs.Select(x => new ItemLogDTO
            {
                ItemLogId = x.ItemLogId,

                ItemId = x.ItemId,
                ItemName = x.Item.Name,

                EmployeeName = x.Employee != null
                    ? x.Employee.FullName
                    : null,

                ActionType = x.ActionType.ToString(),

                Quantity = x.Quantity,
                EntitlementYear = x.EntitlementYear,

                Notes = x.Notes,

                CreatedBy = x.CreatedBy.Username,
                CreatedDate = x.CreatedDate.ToString("yyyy-MM-dd")
            })
            .ToList();
        }
    }

}
