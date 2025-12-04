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
        public static IEnumerable<MatrixDTO> GetAllMatrices()
        {
            return MatrixRepo.GetAllMatrices().Select(m => new MatrixDTO
            {
                Category = m.Matrix.Category.Name,
                Item = m.Item.Name,
                Quantity = m.Quantity,
                Frequency = m.Frequency
            });
        }
    }
}
