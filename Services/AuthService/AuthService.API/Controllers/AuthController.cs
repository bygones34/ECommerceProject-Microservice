using AuthService.Application.DTOs;
using AuthService.Application.Exceptions;
using AuthService.Application.Services;
using AuthService.Application.Services.Blacklist;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Azure.Core;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IBlacklistService _blacklistService;

        public AuthController(IUserService userService, IBlacklistService blacklistService)
        {
            _userService = userService;
            _blacklistService = blacklistService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
        {
            var result = await _userService.AuthenticateAsync(loginDto.Username, loginDto.Password);
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid credentials!" });
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto)
        {
            try
            {
                var user = new User
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                    Role = registerDto.Role ?? "User"
                };

                await _userService.RegisterUserAsync(user, registerDto.Password);

                return StatusCode(201, new { message = "User created successfully." });
            }
            catch (UserAlreadyExistsException ex)
            {
                return Conflict(new {Error = ex.Message});
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "An unexpected error occured!" });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var result = await _userService.RefreshTokenAsync(refreshTokenDto.UserName, refreshTokenDto.RefreshToken);
            if (result == null)
                return Unauthorized("Invalid user or refresh token!");

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync([FromBody] LogoutDto logoutDto)
        {
            var authHeader = Request.Headers.Authorization.ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return BadRequest("Authorization header missing or invalid.");
            }
            var token = authHeader.Replace("Bearer ", "");
            var expiration = _userService.GetTokenExpiration(token);

            await _blacklistService.AddToBlacklistAsync(token, expiration);
            await _userService.LogoutAsync(logoutDto.UserName);
            return Ok("Logged out and token blacklisted.");
        }
        
        [HttpGet("is-blacklisted")]
        [AllowAnonymous]
        public async Task<IActionResult> IsBlacklistedAsync([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required.");

            var isBlacklisted = await _blacklistService.IsTokenBlacklistedAsync(token);
            return Ok(isBlacklisted);
        }
    }
}