using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUserNameAsync(string userName);
        Task AddUserAsync(User user);
        Task<bool> ValidateUserCredentialsAsync(string userName, string password);
        Task UpdateUserAsync(User user);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
        Task<bool> IsTokenBlacklistedAsync(string token);
        Task AddToBlacklistAsync(string token, DateTime expiration);
    }
}