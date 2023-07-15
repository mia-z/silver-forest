using SilverForest.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverForest.Infrastructure.Postgres.Abstraction;

public interface IUsersRepository
{
    Task<IEnumerable<User>> GetUsers();
    Task<User> GetUserById(int id);
    Task<bool> UserExists(string email);
    Task<int> CreateUser(User user);
    Task<int> CreateUser(string email, string name);
}
