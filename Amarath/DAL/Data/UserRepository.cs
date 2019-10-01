using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amarath.DAL.Models;
using Amarath.Models;

namespace Amarath.DAL.Data
{
    public class UserRepository : IUserRepository
    {
        private AmarathContext context;

        public User Add(User user)
        {
            throw new NotImplementedException();
        }

        public User Delete(int UserID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public User GetUserByID(int UserID)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public User Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
