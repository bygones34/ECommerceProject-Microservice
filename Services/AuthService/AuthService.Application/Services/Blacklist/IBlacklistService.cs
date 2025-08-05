using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services.Blacklist
{
    public interface IBlacklistService
    {
        Task<bool> IsTokenBlacklistedAsync(string token);
        Task AddToBlacklistAsync(string token, DateTime expiration);
    }
}
