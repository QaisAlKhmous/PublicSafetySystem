using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace PublicSafety.Repositories.Repositories
{
    public static class ItemLogRepo
    {
        public static void Add(
            AppDbContext context,
            ItemLog log)
        {
            context.ItemLogs.Add(log);
        }

        public static List<ItemLog> GetItemLogsByItemId(Guid itemId)
        {
            using (var context = new AppDbContext())
            {
                return context.ItemLogs
                    .Include(x => x.Item)
                    .Include(x => x.Employee)
                    .Include(x => x.CreatedBy)
                    .Where(x => x.ItemId == itemId)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList();
            }
        }

    }
}
