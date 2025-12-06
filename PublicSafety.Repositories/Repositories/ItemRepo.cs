using PublicSafety.Domain.Entities;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Repositories.Repositories
{
    public class ItemRepo
    {

        public static IEnumerable<Item> GetAllItems()
        {
            using (var context = new AppDbContext())
            {
                return context.Items.Include(i => i.AddedBy).Where(i => i.IsActive).ToList();
            }
        }

        public static void AddItem(Item newItem)
        {
            using (var context = new AppDbContext())
            {
                 context.Items.Add(newItem);
                 context.SaveChanges();
            }
        }

        public static void DeleteItem(Guid id)
        {
            using(var context = new AppDbContext())
            {
                var item = context.Items.Find(id);
                item.IsActive = false;

                context.SaveChanges();
            }
        }

        public static void IncreaseItemQuantity(Guid id,int newQuantity)
        {
            using(var context = new AppDbContext())
            {
                var item = context.Items.Find(id);
                item.Quantity += newQuantity;
                context.SaveChanges();
            }
        }

        public static void DecreaseItemQuantity(Guid id, int newQuantity)
        {
            using (var context = new AppDbContext())
            {
                var item = context.Items.Find(id);
                item.Quantity -= newQuantity;
                context.SaveChanges();
            }
        }
        public static Item GetItemById(Guid ItemId)
        {
            using (var context = new AppDbContext())
            {
                var item = context.Items.Include(i => i.AddedBy).FirstOrDefault(i => i.ItemId == ItemId);
                return item;
            }
        }
    }
}
