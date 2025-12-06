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
    public class MatrixService
    {
        //public static IEnumerable<MatrixDTO> GetAllMatrices()
        //{
        //    return MatrixRepo.GetAllMatrices().Select(m => new MatrixDTO
        //    {
        //        Category = m.Matrix.Category.Name,
        //        Item = m.Item.Name,
        //        Quantity = m.Quantity,
        //        Frequency = m.Frequency
        //    });
        //}

        public static MatrixDTO GetMatrixByCategory(Guid CategoryId)
        {
            var matrix = MatrixRepo.GetMatrixByCategory(CategoryId);
            if (matrix == null)
                return null;

            return new MatrixDTO() { CategoryId = matrix.CategoryId, MatrixId = matrix.MatrixId, IsActive = matrix.IsActive, Version = matrix.Version };
        }

        public static IEnumerable<MatrixItemDTO> GetItemsByMatrix(Guid MatrixId)
        {
            var items = MatrixRepo.GetItemsByMatrix(MatrixId);

            return items.Select(i => new MatrixItemDTO()
            {
                MatrixId = MatrixId,
                ItemId = i.ItemId,
                ItemName = i.Item.Name,
                Frequency = i.Frequency,
                MatrixItemId = i.MatrixItemId,
                Quantity = i.Quantity,
                CreatedBy = i.CreatedBy.Username
            });
        }

        public static IEnumerable<MatrixItemDTO> GetItemsByCategory(Guid CategoryId)
        {
            var matrix = GetMatrixByCategory(CategoryId);

            if (matrix == null)
                return null;

            return GetItemsByMatrix(matrix.MatrixId);
        }

        public static bool IsMatrixExistsForCategory(Guid CategoryId)
        {
            return MatrixRepo.IsMatrixExistsForCategory(CategoryId);
        }

        public static Guid AddNewMatrix(Guid CategoryId)
        {
            var matrix = GetMatrixByCategory(CategoryId);
            Guid id = Guid.NewGuid();
            if(matrix != null)
            {
                id = MatrixRepo.AddNewMatrix(CategoryId, matrix.Version + 1);
                MatrixRepo.DeactivateMatrix(matrix.MatrixId);
            }
            else
            {
                id = MatrixRepo.AddNewMatrix(CategoryId, 1);
            }
            return id;
        }

        public static Guid AddNewMatrixItem(MatrixItemDTO matrixItem)
        {
            var newMatrixItem = new Domain.Entities.MatrixItem()
            {
                ItemId = matrixItem.ItemId,
                MatrixId = matrixItem.MatrixId,
                CreatedById = UserService.GetUserByUsername(matrixItem.CreatedBy).UserId,
                Frequency = matrixItem.Frequency,
                Quantity = matrixItem.Quantity,
                MatrixItemId = Guid.NewGuid(),
                CreatedDate = DateTime.Now
            };
            return MatrixRepo.AddNewItemInMatrix(newMatrixItem);
        }

        public static void DeleteMatrixItem(Guid MatrixItemId)
        {
            MatrixRepo.DeleteMatrixItem(MatrixItemId);

        }
        public static MatrixItemDTO GetMatrixItemById(Guid MatrixItemId)
        {
            var matrixItem = MatrixRepo.GetMatrixItemById(MatrixItemId);
            return new MatrixItemDTO() { ItemId = matrixItem.ItemId, 
                MatrixItemId = matrixItem.MatrixItemId,
                Frequency = matrixItem.Frequency,
                Quantity = matrixItem.Frequency};

        }

        public static void UpdateMatrixItem(UpdateMatrixItemDTO matrixItem)
        {
            var exMatrixItem = MatrixRepo.GetMatrixItemById(matrixItem.MatrixItemId);
            exMatrixItem.Frequency = matrixItem.Frequency;
            exMatrixItem.Quantity = matrixItem.Quantity;

            MatrixRepo.UpdateMatrixItem(exMatrixItem);

        }
    }
}
