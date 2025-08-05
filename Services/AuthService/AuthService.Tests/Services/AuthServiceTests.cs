using AuthService.Application.DTOs;
using AuthService.Application.Exceptions;
using AuthService.Application.Services;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AuthService.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IConfiguration> _configurationMock = new();
        private readonly UserService _userService;

        public AuthServiceTests()
        {
            _configurationMock
                .Setup(c => c["Jwt:Key"])
                .Returns("ThisIsASecretKeyForTestingPurposesOnly");

            _userService = new UserService(_userRepositoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_SHouldReturnUserId_WhenRegistrationSuccessful()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testUser",
                Email = "test@example.com"
            };
            var password = "P@ssw0rd";

            _userRepositoryMock.Setup(repo => repo.GetByUserNameAsync(user.UserName))
                .ReturnsAsync((User?)null);

            // Act
            await _userService.RegisterUserAsync(user, password);

            // Assert
            _userRepositoryMock.Verify(repo => repo.AddUserAsync(It.Is<User>(u => u.UserName == user.UserName)), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldThrowException_WhenUserAlreadyExists()
        {
            // Arrange
            var existingUser = new User { UserName = "testuser" };

            _userRepositoryMock
                .Setup(repo => repo.GetByUserNameAsync(existingUser.UserName))
                .ReturnsAsync(existingUser); // Kullanıcı var

            var user = new User
            {
                UserName = "testuser",
                Email = "test@example.com",
            };
            var password = "P@ssw0rd";

            // Act & Assert
            await Assert.ThrowsAsync<UserAlreadyExistsException>(() =>
                _userService.RegisterUserAsync(user, password));
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldHashPasswordCorrectly()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "secureuser",
                Email = "secure@example.com"
            };
            var plainPassword = "securepassword";

            _userRepositoryMock
                .Setup(repo => repo.GetByUserNameAsync(user.UserName))
                .ReturnsAsync((User?)null); 

            _userRepositoryMock
                .Setup(repo => repo.AddUserAsync(It.IsAny<User>()))
                .Callback<User>(addedUser =>
                {
                    Assert.True(BCrypt.Net.BCrypt.Verify(plainPassword, addedUser.PasswordHash));
                })
                .Returns(Task.CompletedTask);

            var inMemorySettings = new Dictionary<string, string>
            {
                {"JwtSettings:SecretKey", "THIS_IS_A_TEST_SECRET_KEY_123456"},
                {"JwtSettings:Issuer", "TestIssuer"},
                {"JwtSettings:Audience", "TestAudience"},
                {"JwtSettings:ExpiryMinutes", "60"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var userService = new UserService(_userRepositoryMock.Object, configuration);

            // Act
            await userService.RegisterUserAsync(user, plainPassword);

            // Assert
            _userRepositoryMock.Verify(repo => repo.AddUserAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
                Email = "test@example.com",
                Role = "User"
            };

            _userRepositoryMock
                .Setup(repo => repo.ValidateUserCredentialsAsync(user.UserName, "password"))
                .ReturnsAsync(true);

            _userRepositoryMock
                .Setup(repo => repo.GetByUserNameAsync(user.UserName))
                .ReturnsAsync(user);

            var inMemorySettings = new Dictionary<string, string>
            {
                {"JwtSettings:SecretKey", "THIS_IS_A_TEST_SECRET_KEY_123456"},
                {"JwtSettings:Issuer", "TestIssuer"},
                {"JwtSettings:Audience", "TestAudience"},
                {"JwtSettings:ExpiryMinutes", "60"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var userService = new UserService(_userRepositoryMock.Object, configuration);

            // Act
            var token = await userService.AuthenticateAsync(user.UserName, "password");

            // Assert
            Assert.NotNull(token);
            Assert.IsType<AuthResponseDto>(token);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
        {
            // Arrange
            var username = "invaliduser";
            var password = "wrongpassword";

            _userRepositoryMock
                .Setup(repo => repo.ValidateUserCredentialsAsync(username, password))
                .ReturnsAsync(false);

            var inMemorySettings = new Dictionary<string, string>
            {
                {"JwtSettings:SecretKey", "THIS_IS_A_TEST_SECRET_KEY_123456"},
                {"JwtSettings:Issuer", "TestIssuer"},
                {"JwtSettings:Audience", "TestAudience"},
                {"JwtSettings:ExpiryMinutes", "60"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var userService = new UserService(_userRepositoryMock.Object, configuration);

            // Act
            var result = await userService.AuthenticateAsync(username, password);

            // Assert
            Assert.Null(result);
        }
    }
}
