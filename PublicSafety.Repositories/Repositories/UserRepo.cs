using PublicSafety.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Repositories.Repositories
{
    public class UserRepo
    {

        public static User GetUserByUsername(string username)
        {
            using (var context = new AppDbContext())
            {
                return context.Users.Where(u => u.Username == username).FirstOrDefault();
            }
        }

        public static User GetUserById(Guid Id)
        {
            using (var context = new AppDbContext())
            {
                return context.Users.Find(Id);
            }
        }

        public static IEnumerable<User> GetAdmins()
        {
            using (var context = new AppDbContext())
            {
                return context.Users.Where(u => u.Type == enType.Admin).ToList();
            }
        }


    }
}
