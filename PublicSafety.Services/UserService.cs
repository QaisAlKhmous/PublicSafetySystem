using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services
{
    public class UserService
    {

        public static UserDTO GetUserByUsername(string Username)
        {
            var user = UserRepo.GetUserByUsername(Username);
            return new UserDTO() { UserId = user.UserId,Username = user.Username};
        }
    }
}
