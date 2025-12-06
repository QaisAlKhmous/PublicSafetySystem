using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Repositories.Repositories
{
    public class MatrixRepo
    {
        public static IEnumerable<MatrixItem> GetAllMatrices()
        {
            using (var context = new AppDbContext())
            {
                return context.MatrixItems.Include(c => c.Matrix.Category).Include(m => m.Item).ToList();
            }
        }

        public static MatrixItem GetMatrixItemById(Guid MatrixItemId)
        {
            using (var context = new AppDbContext())
            {
                return context.MatrixItems.Find(MatrixItemId);
            }
        }

        public static void UpdateMatrix(Matrix matrix)
        {
            using (var context = new AppDbContext())
            {
                var updatedMatrix = context.Matrices.Find(matrix.MatrixId);

                updatedMatrix.CategoryId = matrix.CategoryId;
                updatedMatrix.Version = matrix.Version;
                updatedMatrix.IsActive = matrix.IsActive;

                context.SaveChanges();
            }
        }

        public static void DeactivateMatrix(Guid MatrixId)
        {
            using (var context = new AppDbContext())
            {
                var matrix = context.Matrices.Find(MatrixId);
                matrix.IsActive = false;

                context.SaveChanges();
            }
        }
        public static Matrix GetMatrixByCategory(Guid CategoryId)
        {
            using(var context = new AppDbContext())
            {
                return context.Matrices.FirstOrDefault(m => m.CategoryId == CategoryId && m.IsActive == true);
            }
        }

        public static IEnumerable<MatrixItem> GetItemsByMatrix(Guid MatrixId)
        {
            using (var context = new AppDbContext())
            {
                return context.MatrixItems.Include(m => m.Item).Include(m => m.CreatedBy).Where(m => m.MatrixId == MatrixId).ToList();
            }
        }

        public static bool IsMatrixExistsForCategory(Guid CategoryId)
        {
            using (var context = new AppDbContext())
            {
                return context.Matrices.Any(m => m.CategoryId == CategoryId && m.IsActive == true);
            }
        }

        public static Guid AddNewMatrix(Guid CategoryId,int Version)
        {
            using (var context = new AppDbContext())
            {
                var matrix = new Matrix() { CategoryId = CategoryId, MatrixId = Guid.NewGuid(), IsActive = true, Version = Version };
               context.Matrices.Add(matrix);
                context.SaveChanges();

                return matrix.MatrixId;
            }
        }

        public static Guid AddNewItemInMatrix(MatrixItem matrixItem)
        {
            using (var context = new AppDbContext())
            {


                var newMatrixItem = context.MatrixItems.Add(matrixItem);
                context.SaveChanges();

                return newMatrixItem.MatrixItemId;
            }
        }

        public static void DeleteMatrixItem(Guid MatrixItemId)
        {
            using (var context = new AppDbContext())
            {
                var matrixItem = context.MatrixItems.Find(MatrixItemId);

                context.MatrixItems.Remove(matrixItem);

                context.SaveChanges();

            }
        }

        public static void UpdateMatrixItem(MatrixItem matrixItem)
        {
            using (var context = new AppDbContext())
            {
                var updatedMatrixItem = context.MatrixItems.Find(matrixItem.MatrixItemId);
                updatedMatrixItem.Quantity = matrixItem.Quantity;
                updatedMatrixItem.Frequency = matrixItem.Frequency;

                context.SaveChanges();

            }
        }

    }
}
