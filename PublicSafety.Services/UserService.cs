using AdminDashboard.Services;
using PublicSafety.Domain.Entities;
using PublicSafety.Repositories.Repositories;
using PublicSafety.Services.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            return new UserDTO() { UserId = user.UserId,Username = user.Username,Type = (int)user.Type};
        }

        public static UserDTO GetUserByUserId(Guid UserId)
        {
            var user = UserRepo.GetUserById(UserId);
            return new UserDTO() { UserId = user.UserId, Username = user.Username };
        }

        public static IEnumerable<UserDTO> GetAdmins()
        {
            var admins = UserRepo.GetAdmins();

            return admins.Select(user => new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Type = (int)user.Type,
                Email = user.Email
            });
        }

        public static LoginResultDTO Login(string Username,string Password)
        {
            var user = UserRepo.GetUserByUsername(Username);

            if (user == null)
            {
                return null;
            }

            if (Password == user.PasswordHash)
            {
                return new LoginResultDTO
                {
                    UserId = user.UserId,
                    Username = Username,
                    Type = (int)user.Type,
                    IsPassword = true
                };
            }



            return new LoginResultDTO
            {
                UserId = user.UserId,
                Username = Username,
                Type = (int)user.Type,
                IsPassword = false
            };
        }
    }
}
