using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amarath.DAL.Models;

namespace Amarath.DAL.Data
{
    public interface IUserRepository
    {
        User GetUserByID(int UserID);
        User Add(User user);
        User Update(User user);
        User Delete(int UserID);
        IEnumerable<User> GetAllUsers();
        void Save();
    }
}
