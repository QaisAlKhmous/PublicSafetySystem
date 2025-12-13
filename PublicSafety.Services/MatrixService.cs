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

        public static Guid CreateNewMatrixVersion(Guid categoryId)
        {
            using (var context = new AppDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var currentMatrix = context.Matrices
                        .Where(m => m.CategoryId == categoryId && m.ValidTo == null)
                        .FirstOrDefault();

                    int newVersion = 1;

                    if (currentMatrix != null)
                    {
                        newVersion = currentMatrix.Version + 1;
                       MatrixRepo.DeactivateMatrix(currentMatrix.MatrixId, context);
                    }

                    var newMatrix =MatrixRepo.CreateNewMatrix(categoryId, newVersion, context);

                    context.SaveChanges();
                    transaction.Commit();

                    return newMatrix.MatrixId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        public static Guid AddNewMatrixItem(MatrixItemDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var user = UserService.GetUserByUsername(dto.CreatedBy);
            if (user == null)
                throw new Exception("Invalid user");

            return MatrixRepo.CreateNewMatrixVersionWithNewItem(
                dto.MatrixId,
                dto.ItemId,
                dto.Quantity,
                dto.Frequency,
                user.UserId
            );
        }

        public static void DeleteMatrixItem(Guid matrixItemId)
        {
            var matrixItem = MatrixRepo.GetMatrixItemById(matrixItemId);
            if (matrixItem == null)
                throw new Exception("Matrix item not found");

            MatrixRepo.CreateNewMatrixVersionWithoutItem(matrixItemId);
        }
        public static MatrixItemDTO GetMatrixItemById(Guid MatrixItemId)
        {
            var matrixItem = MatrixRepo.GetMatrixItemById(MatrixItemId);
            return new MatrixItemDTO() { ItemId = matrixItem.ItemId, 
                MatrixItemId = matrixItem.MatrixItemId,
                Frequency = matrixItem.Frequency,
                Quantity = matrixItem.Frequency};

        }

        public static void UpdateMatrixItem(UpdateMatrixItemDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var matrixItem = MatrixRepo.GetMatrixItemById(dto.MatrixItemId);
            if (matrixItem == null)
                throw new Exception("Matrix item not found");

            
            MatrixRepo.CreateNewMatrixVersionWithUpdatedItem(
                dto.MatrixItemId,
                dto.Quantity,
                dto.Frequency
            );
        }
    }
}
