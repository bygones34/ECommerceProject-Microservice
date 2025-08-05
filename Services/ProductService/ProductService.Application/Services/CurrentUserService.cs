using Microsoft.AspNetCore.Http;
using ProductService.Application.Interfaces;
using System.Security.Claims;

namespace ProductService.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public string? UserName { get; }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserName = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
