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

        public static void CreateNewMatrixVersionWithUpdatedItem(
    Guid matrixItemId,
    int newQuantity,
    int newFrequency)
        {
            using (var context = new AppDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // 1️⃣ Load matrix item + matrix
                    var oldMatrixItem = context.MatrixItems
                        .Include(mi => mi.Matrix)
                        .FirstOrDefault(mi => mi.MatrixItemId == matrixItemId);

                    if (oldMatrixItem == null)
                        throw new Exception("Matrix item not found");

                    var oldMatrix = oldMatrixItem.Matrix;

                    // 2️⃣ Deactivate old matrix
                    oldMatrix.IsActive = false;
                    oldMatrix.ValidTo = DateTime.Now;
                    context.Entry(oldMatrix).State = EntityState.Modified;

                    // 3️⃣ Create new matrix version
                    var newMatrix = new Matrix
                    {
                        MatrixId = Guid.NewGuid(),
                        CategoryId = oldMatrix.CategoryId,
                        Version = oldMatrix.Version + 1,
                        IsActive = true,
                        ValidFrom = DateTime.Now,
                        ValidTo = null
                    };

                    context.Matrices.Add(newMatrix);

                    // 4️⃣ Copy all matrix items
                    var oldItems = context.MatrixItems
                        .Where(mi => mi.MatrixId == oldMatrix.MatrixId)
                        .ToList();

                    foreach (var oldItem in oldItems)
                    {
                        var newItem = new MatrixItem
                        {
                            MatrixItemId = Guid.NewGuid(),
                            MatrixId = newMatrix.MatrixId,
                            ItemId = oldItem.ItemId,

                            Quantity = oldItem.MatrixItemId == matrixItemId
                                ? newQuantity
                                : oldItem.Quantity,

                            Frequency = oldItem.MatrixItemId == matrixItemId
                                ? newFrequency
                                : oldItem.Frequency,
                            CreatedById = oldItem.CreatedById,
                            CreatedDate = DateTime.Now
                        };

                        context.MatrixItems.Add(newItem);
                    }

                    // 5️⃣ Persist
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

        public static Guid CreateNewMatrixVersionWithNewItem(
    Guid oldMatrixId,
    Guid itemId,
    int quantity,
    int frequency,
    Guid createdById)
        {
            using (var context = new AppDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // 1️⃣ Load old matrix
                    var oldMatrix = context.Matrices
                        .FirstOrDefault(m => m.MatrixId == oldMatrixId);

                    if (oldMatrix == null)
                        throw new Exception("Matrix not found");

                    // 2️⃣ Deactivate old matrix
                    oldMatrix.IsActive = false;
                    oldMatrix.ValidTo = DateTime.Now;
                    context.Entry(oldMatrix).State = EntityState.Modified;

                    // 3️⃣ Create new matrix version
                    var newMatrix = new Matrix
                    {
                        MatrixId = Guid.NewGuid(),
                        CategoryId = oldMatrix.CategoryId,
                        Version = oldMatrix.Version + 1,
                        IsActive = true,
                        ValidFrom = DateTime.Now,
                        ValidTo = null

                    };

                    context.Matrices.Add(newMatrix);

                    // 4️⃣ Copy old matrix items
                    var oldItems = context.MatrixItems
                        .Where(mi => mi.MatrixId == oldMatrix.MatrixId)
                        .ToList();

                    foreach (var oldItem in oldItems)
                    {
                        var newItem = new MatrixItem
                        {
                            MatrixItemId = Guid.NewGuid(),
                            MatrixId = newMatrix.MatrixId,
                            ItemId = oldItem.ItemId,
                            Quantity = oldItem.Quantity,
                            Frequency = oldItem.Frequency,
                            CreatedDate = DateTime.Now,
                            CreatedById = createdById
                        };

                        context.MatrixItems.Add(newItem);
                    }

                    // 5️⃣ Add the NEW matrix item
                    var addedItem = new MatrixItem
                    {
                        MatrixItemId = Guid.NewGuid(),
                        MatrixId = newMatrix.MatrixId,
                        ItemId = itemId,
                        Quantity = quantity,
                        Frequency = frequency,
                        CreatedDate = DateTime.Now,
                        CreatedById = createdById
                    };

                    context.MatrixItems.Add(addedItem);

                    // 6️⃣ Save & commit
                    context.SaveChanges();
                    transaction.Commit();

                    return addedItem.MatrixItemId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public static void CreateNewMatrixVersionWithoutItem(Guid matrixItemId)
        {
            using (var context = new AppDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // 1️⃣ Load matrix item + matrix
                    var matrixItem = context.MatrixItems
                        .Include(mi => mi.Matrix)
                        .FirstOrDefault(mi => mi.MatrixItemId == matrixItemId);

                    if (matrixItem == null)
                        throw new Exception("Matrix item not found");

                    var oldMatrix = matrixItem.Matrix;

                    // 2️⃣ Deactivate old matrix
                    oldMatrix.IsActive = false;
                    oldMatrix.ValidTo = DateTime.Now;
                    context.Entry(oldMatrix).State = EntityState.Modified;

                    // 3️⃣ Create new matrix version
                    var newMatrix = new Matrix
                    {
                        MatrixId = Guid.NewGuid(),
                        CategoryId = oldMatrix.CategoryId,
                        Version = oldMatrix.Version + 1,
                        IsActive = true,
                        ValidFrom = DateTime.Now,
                        ValidTo = null

                    };

                    context.Matrices.Add(newMatrix);

                    // 4️⃣ Copy all matrix items EXCEPT the deleted one
                    var oldItems = context.MatrixItems
                        .Where(mi => mi.MatrixId == oldMatrix.MatrixId)
                        .ToList();

                    foreach (var oldItem in oldItems)
                    {
                        if (oldItem.MatrixItemId == matrixItemId)
                            continue; // 🚫 skip deleted item

                        var newItem = new MatrixItem
                        {
                            MatrixItemId = Guid.NewGuid(),
                            MatrixId = newMatrix.MatrixId,
                            ItemId = oldItem.ItemId,
                            Quantity = oldItem.Quantity,
                            Frequency = oldItem.Frequency,
                            CreatedDate = DateTime.Now,
                            CreatedById = matrixItem.CreatedById
                        };

                        context.MatrixItems.Add(newItem);
                    }

                    // 5️⃣ Save & commit
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
