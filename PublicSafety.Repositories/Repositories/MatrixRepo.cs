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

        public static void UpdateMatrix(Matrix matrix)
        {
            using (var context = new AppDbContext())
            {
                //var matrix = context.Matrices.FirstOrDefault(matrix.MatrixId);
            }
        }

    }
}
