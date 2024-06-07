using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.User;
using Domain.User.ValueObjects;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _dbContext;

        public UserRepository(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public User? GetUserByUsername(string username)
        {
            return _dbContext.Users.SingleOrDefault(u => u.Username == username);
        }

        public User? GetUserByEmailAndPassword(string email, string password)
        {
            var user = _dbContext.Users
            .Include(u => u.Password)
            .SingleOrDefault(u => u.Email == email);

            if (user != null && user.Password.Verify(password))
            {
                return user;
            }

            return null;
        }

        public User? GetUserByEmail(string email)
        {
            return _dbContext.Users.SingleOrDefault(u => u.Email == email);
        }

        public User? GetUserById(string id)
        {
            return _dbContext.Users.SingleOrDefault(u => u.Id.ToString() == id);
        }

        public void Add(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        public void Save(User user)
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
        }
    }
}