using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SilverForest.Common.Models;
using SilverForest.Infrastructure.Postgres.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SilverForest.Infrastructure.Postgres.Services;

internal class UsersRepository : IUsersRepository
{
    private readonly SilverForestDbContext _context;
    private readonly ILogger<UsersRepository> _logger;

    public UsersRepository(SilverForestDbContext context, ILogger<UsersRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> CreateUser(User user)
    {
        _logger.LogInformation($"Creating new user: {user.Name}, {user.Email}");
        try
        {
            var newUser = await _context.Users.AddAsync(new User
            {
                Email = user.Email,
                Name = user.Name
            });

            var rows = await _context.SaveChangesAsync();

            if (rows > 0)
            {
                _logger.LogInformation($"Created new user: {user.Name}, {user.Email}");
                return newUser.Entity.Id;
            }
            else
            {
                _logger.LogError($"Error creating new user: {user.Name}, {user.Email}\nGot no new rows!");
                return -1;
            }
        } catch (Exception ex)
        {
            _logger.LogError("Exception raised when trying to create user:\n" + ex.Message);
            return -1;
        }
    }

    public async Task<int> CreateUser(string email, string name)
    {
        _logger.LogInformation($"Creating new user: {name}, {email}");

        try
        {
            var newUser = await _context.Users.AddAsync(new User
            {
                Email = email,
                Name = name
            });

            var rows = await _context.SaveChangesAsync();

            if (rows < 1)
            {
                _logger.LogInformation($"Created new user: {name}, {email}");
                return newUser.Entity.Id;
            }
            else
            {
                _logger.LogError($"Error creating new user: {name}, {email}\nGot no new rows!");
                return -1;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception raised when trying to create user:\n" + ex.Message);
            return -1;
        }
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _context.Users.AsNoTracking().ToListAsync();
    }

    public async Task<bool> UserExists(string email)
    {
        return await _context.Users.AsNoTracking().AnyAsync(u => u.Email == email);
    }
}
