using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Tests.Repositories
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task GetByUserNameAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AuthDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var testUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("test123"),
                Email = "test@example.com",
                Role = "User"
            };

            using (var context = new AuthDbContext(options))
            {
                context.Users.Add(testUser);
                context.SaveChanges();
            }

            using (var context = new AuthDbContext(options))
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetByUserNameAsync("testuser");

                // Assert
                Assert.NotNull(result);
                Assert.Equal("testuser", result.UserName);
            }
        }

        [Fact]
        public async Task ValidateUserCredentialsAsync_ShouldReturnTrue_WhenCredentialsAreValid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AuthDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var password = "secure123";
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "validuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Email = "valid@example.com",
                Role = "User"
            };

            using (var context = new AuthDbContext(options))
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            using (var context = new AuthDbContext(options))
            {
                var repository = new UserRepository(context);

                // Act
                var isValid = await repository.ValidateUserCredentialsAsync("validuser", password);

                // Assert
                Assert.True(isValid);
            }
        }

        [Fact]
        public async Task ValidateUserCredentialsAsync_ShouldReturnFalse_WhenPasswordIsInvalid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AuthDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "wrongpassuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPass123"),
                Email = "wrongpass@example.com"
            };

            using (var context = new AuthDbContext(options))
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            using (var context = new AuthDbContext(options))
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.ValidateUserCredentialsAsync("wrongpassuser", "WrongPassword!");

                // Assert
                Assert.False(result);
            }
        }
    }

}
