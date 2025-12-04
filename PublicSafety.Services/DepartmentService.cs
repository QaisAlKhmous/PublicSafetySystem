using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public class DepartmentService
    {

        public static IEnumerable<DepartmentDTO> GetAllDepartments()
        {
            var categories = DepartmentRepo.GetAllDepartments();

            return categories.Select(c => new DepartmentDTO()
            {
                DepartmentId = c.DepartmentId,
                Name = c.Name,
            });
        }

        public static DepartmentDTO GetDepartmentByName(string name)
        {
            var Department = DepartmentRepo.GetDepartmentByName(name);

            return new DepartmentDTO { DepartmentId = Department.DepartmentId, Name = name };   
        }
    }
}
