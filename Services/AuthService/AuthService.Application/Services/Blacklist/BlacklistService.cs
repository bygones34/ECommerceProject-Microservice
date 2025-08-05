using AuthService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Blacklist
{
    public class BlacklistService : IBlacklistService
    {
        private readonly IUserRepository _userRepository;

        public BlacklistService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task AddToBlacklistAsync(string token, DateTime expiration)
        {
            await _userRepository.AddToBlacklistAsync(token, expiration);
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            return await _userRepository.IsTokenBlacklistedAsync(token);
        }
    }
}
