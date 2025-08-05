using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _authDbContext;

        public UserRepository(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public async Task AddUserAsync(User user)
        {
            await _authDbContext.Users.AddAsync(user);
            await _authDbContext.SaveChangesAsync();
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _authDbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<bool> ValidateUserCredentialsAsync(string userName, string password)
        {
            var user = await GetByUserNameAsync(userName);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public async Task UpdateUserAsync(User user)
        {
            _authDbContext.Users.Update(user);
            await _authDbContext.SaveChangesAsync();
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _authDbContext.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            return await _authDbContext.BlacklistedTokens.AnyAsync(t => t.Token == token);
        }

        public async Task AddToBlacklistAsync(string token, DateTime expiration)
        {
            _authDbContext.BlacklistedTokens.Add(new BlacklistedToken
            {
                Token = token,
                Expiration = expiration
            });

            await _authDbContext.SaveChangesAsync();
        }
    }
}
