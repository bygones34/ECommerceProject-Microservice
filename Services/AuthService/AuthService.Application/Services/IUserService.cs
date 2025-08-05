using AuthService.Application.DTOs;
using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services
{
    public interface IUserService
    {
        Task<AuthResponseDto?> AuthenticateAsync(string username, string password);
        Task RegisterUserAsync(User user, string password);
        Task<AuthResponseDto?> RefreshTokenAsync(string userName, string refreshToken);
        Task LogoutAsync(string userName);
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
        DateTime GetTokenExpiration(string token);
    }
}