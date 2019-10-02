using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amarath.DAL.Models;
using Amarath.Models;

//Same thing as a UserRepository or IUserRepository
namespace Amarath.DAL.Data
{
    public interface IUserService
    {
        User Authentication(string username, string password);
        User GetUserByID(int UserID);
        User Create(User user);
        User Update(User user);
        User Delete(int UserID);
        IEnumerable<User> GetAllUsers();
        void Save();
    }
    public class UserService : IUserService
    {
        private AmarathContext context;

        public User Create(User user)
        {
            throw new NotImplementedException();
        }

        public User Authentication(string username, string password)
        {
            var user = new User();

            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }
            //find user matching the supplied username
            user = context.Users.SingleOrDefault(x => x.UserName == username);
            if(user == null)
            {
                return null;
            }

            //TODO: Password verification
            return user;
        }

        public User Delete(int UserID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAllUsers()
        {
            return context.Users.ToList();
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
