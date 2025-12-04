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
                return context.Items.Include(i => i.AddedBy).ToList();
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
                context.Items.Remove(item);

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
    }
}
