using PublicSafety.Domain.Entities;
using PublicSafety.Repositories;
using PublicSafety.Repositories.Migrations;
using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public class ItemService
    {
        public static IEnumerable<ItemsDTO> GetAllItems()
        {
            var items = ItemRepo.GetAllItems();
            return items.Select(i => new ItemsDTO
            {
                ItemId = i.ItemId,
                Name = i.Name,
                Description = i.Description,
                Quantity = i.Quantity,
                IsActive = i.IsActive,
                AddedBy = i.AddedBy.Username
            });
        }

        public static void AddItem(ItemsDTO item)
        {
            var newItem = new Item() { ItemId = Guid.NewGuid(),Name = item.Name, Description = item.Description, Quantity = item.Quantity, IsActive = true, AddedById =  UserService.GetUserByUsername(item.AddedBy).UserId};
            ItemRepo.AddItem(newItem);
        }

        public static void DeleteItem(Guid id)
        {
            ItemRepo.DeleteItem(id);
        }

        public static void IncreaseItemQuantity(Guid id, int AddedQuantity)
        {
            ItemRepo.IncreaseItemQuantity(id, AddedQuantity);
        }

        public static void DecreaseItemQuantity(Guid id, int AddedQuantity)
        {
            ItemRepo.DecreaseItemQuantity(id, AddedQuantity);
        }

        public static ItemsDTO GetItemById(Guid Id)
        {
            var item = ItemRepo.GetItemById(Id);

            return new ItemsDTO()
            {
                ItemId = item.ItemId,
                Name = item.Name,
                Description = item.Description,
                AddedBy = UserService.GetUserByUsername(item.AddedBy.Username).Username,
                Quantity = item.Quantity
            };
        }

        public static bool IsItemExistsByName(string Name)
        {
            return ItemRepo.IsItemExistsByName(Name);
        }
        public static int GetNumberOfAllItems()
        {
            return ItemRepo.GetNumberOfAllItems();
        }

    }
}
