using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

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

        public static void DeactivateMatrix(Guid matrixId, AppDbContext context)
        {
            var matrix = context.Matrices.Find(matrixId);
            if (matrix == null) return;

            matrix.ValidTo = DateTime.Now;
            matrix.IsActive = false; // اختياري
        }
        public static Matrix GetMatrixByCategory(Guid CategoryId)
        {
            using(var context = new AppDbContext())
            {
                return context.Matrices.FirstOrDefault(m => m.CategoryId == CategoryId && m.IsActive == true);
            }
        }

        public static Matrix GetMatrixByMatrixId(Guid MatrixId)
        {
            using (var context = new AppDbContext())
            {
                return context.Matrices.Find(MatrixId);
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

        public static Matrix CreateNewMatrix(Guid categoryId, int version, AppDbContext context)
        {
            var matrix = new Matrix
            {
                MatrixId = Guid.NewGuid(),
                CategoryId = categoryId,
                Version = version,
                ValidFrom = DateTime.Now,
                ValidTo = null,
                IsActive = true // اختياري
            };

            context.Matrices.Add(matrix);
            return matrix;
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
        private static Matrix GetOrCreateMatrixForYear(
     AppDbContext context,
     Guid categoryId,
     int year)
        {
            DateTime validFrom = new DateTime(year, 1, 1);

            // 1️⃣ Matrix already exists for this year
            var matrix = context.Matrices.SingleOrDefault(m =>
                m.CategoryId == categoryId &&
                m.ValidFrom == validFrom
            );

            if (matrix != null)
                return matrix;

            // 2️⃣ Get previous matrix
            var previousMatrix = context.Matrices
                .Where(m => m.CategoryId == categoryId && m.ValidFrom < validFrom)
                .OrderByDescending(m => m.ValidFrom)
                .FirstOrDefault();

            if (previousMatrix == null)
                throw new Exception("No base matrix found");

            // 3️⃣ Close previous matrix
            previousMatrix.ValidTo = validFrom.AddDays(-1);
            previousMatrix.IsActive = false;

            // 4️⃣ Create new matrix for this year
            matrix = new Matrix
            {
                MatrixId = Guid.NewGuid(),
                CategoryId = categoryId,
                Version = previousMatrix.Version + 1,
                ValidFrom = validFrom,
                ValidTo = null,
                IsActive = year == DateTime.Now.Year
            };

            context.Matrices.Add(matrix);

            // 5️⃣ Copy matrix items
            var previousItems = context.MatrixItems
                .Where(mi => mi.MatrixId == previousMatrix.MatrixId)
                .ToList();

            foreach (var item in previousItems)
            {
                context.MatrixItems.Add(new MatrixItem
                {
                    MatrixItemId = Guid.NewGuid(),
                    MatrixId = matrix.MatrixId,
                    ItemId = item.ItemId,
                    Quantity = item.Quantity,
                    Frequency = item.Frequency,
                    CreatedDate = DateTime.Now,
                    CreatedById = item.CreatedById
                });
            }

            context.SaveChanges();
            return matrix;
        }



        public static void UpdateMatrixItemForCurrentYear(
    Guid matrixItemId,
    int newQuantity,
    int newFrequency)
        {
            using (var context = new AppDbContext())
            using (var tx = context.Database.BeginTransaction())
            {
                var oldItem = context.MatrixItems
                    .Include(mi => mi.Matrix)
                    .FirstOrDefault(mi => mi.MatrixItemId == matrixItemId);

                if (oldItem == null)
                    throw new Exception("Matrix item not found");

                int year = DateTime.Now.Year;

                var matrix = GetOrCreateMatrixForYear(
                    context,
                    oldItem.Matrix.CategoryId,
                    year
                );

                var item = context.MatrixItems.SingleOrDefault(mi =>
                    mi.MatrixId == matrix.MatrixId &&
                    mi.ItemId == oldItem.ItemId
                );

                if (item == null)
                    throw new Exception("Matrix item not found in current year");

                item.Quantity = newQuantity;
                item.Frequency = newFrequency;

                context.SaveChanges();
                tx.Commit();
            }
        }



        public static Guid AddItemToMatrixForCurrentYear(
     Guid matrixId,
     Guid itemId,
     int quantity,
     int frequency,
     Guid userId)
        {
            using (var context = new AppDbContext())
            using (var tx = context.Database.BeginTransaction())
            {
                int year = DateTime.Now.Year;



                // 1️⃣ Load matrix to get CategoryId
                var baseMatrix = context.Matrices
                    .Where(m => m.MatrixId == matrixId)
                    .Select(m => new
                    {
                        m.MatrixId,
                        m.CategoryId
                    })
                    .SingleOrDefault();

                if (baseMatrix == null)
                    throw new Exception("Matrix not found");

                // 2️⃣ Get or create matrix for current year using CategoryId
                var matrix = GetOrCreateMatrixForYear(
                    context,
                    baseMatrix.CategoryId,
                    year
                );

                if (context.MatrixItems.Any(mi =>
                    mi.MatrixId == matrix.MatrixId &&
                    mi.ItemId == itemId))
                {
                    throw new Exception("Item already exists in matrix");
                }

                var newItem = new MatrixItem
                {
                    MatrixItemId = Guid.NewGuid(),
                    MatrixId = matrix.MatrixId,
                    ItemId = itemId,
                    Quantity = quantity,
                    Frequency = frequency,
                    CreatedDate = DateTime.Now,
                    CreatedById = userId
                };

                context.MatrixItems.Add(newItem);
                context.SaveChanges();
                tx.Commit();

                return newItem.MatrixItemId;
            }
        }



        public static void RemoveItemFromMatrixForCurrentYear(
    Guid matrixItemId)
        {
            using (var context = new AppDbContext())
            using (var tx = context.Database.BeginTransaction())
            {
                var oldItem = context.MatrixItems
                    .Include(mi => mi.Matrix)
                    .FirstOrDefault(mi => mi.MatrixItemId == matrixItemId);

                if (oldItem == null)
                    throw new Exception("Matrix item not found");

                int year = DateTime.Now.Year;

                var matrix = GetOrCreateMatrixForYear(
                    context,
                    oldItem.Matrix.CategoryId,
                    year
                );

                var item = context.MatrixItems.SingleOrDefault(mi =>
                    mi.MatrixId == matrix.MatrixId &&
                    mi.ItemId == oldItem.ItemId
                );

                if (item == null)
                    throw new Exception("Item not found in current year matrix");

                context.MatrixItems.Remove(item);
                context.SaveChanges();
                tx.Commit();
            }
        }





    }
}
