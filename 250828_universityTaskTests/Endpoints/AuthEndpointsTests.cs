using _250828_universityTask.Auth;
using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Endpoints;
using _250828_universityTask.Logger;
using _250828_universityTask.Models;
using _250828_universityTask.Models.Dtos;
using _250828_universityTask.Models.Requests;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NSubstitute;
using System.Data;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;

namespace _250828_universityTaskTests.Endpoints
{
    [TestClass]
    public class AuthEndpointsTests
    {
        private CacheServiceWithoutExtension _cacheService;
        private IdentityService _identityService;
        private FileLoggerProvider _fileLoggerProvider;

        [TestInitialize]
        public void Setup()
        {
            var _logger = Substitute.For<ILogger<FileLoggerProvider>>();
            _fileLoggerProvider = new FileLoggerProvider(_logger, disableFileIO: true);

            var jsonDb = new JsonDbContext(_fileLoggerProvider)
            {
                Professors = new List<Professor>
            {
                new Professor { Id = 1, Name = "Klaus", UniversityId = 1 }
            },
                Students = new List<Student>
            {
                new Student { Id = 1, Name = "Sophia", UniversityId = 1 }
            }
            };

            var cache = new Cache(_fileLoggerProvider);
            var logger = Substitute.For<ILogger<CacheServiceWithoutExtension>>();
            // var memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheService = new CacheServiceWithoutExtension(jsonDb, cache, logger, _fileLoggerProvider);
            
            var _jwtSettings = new JwtSettings
            {
                Key = "FAKE_KEY_FOR_TEST_PURPOSE_012345",
                Issuer = "TestClass",
                Audiences = new[] { "TestAudience" },
                ExpiryHours = 1
            };

            _identityService = new IdentityService(Options.Create(_jwtSettings), _fileLoggerProvider);
        }

        [TestMethod]
        public void Trying_Login_Should_Return_200()
        {
            // Arrange
            var req = new _250828_universityTask.Models.Requests.LoginRequest(1, "e@e.com", "test", "professor");

            // Act
            var res = AuthEndpoints.LoginLogic(req, _identityService, _cacheService, _fileLoggerProvider);

            // Assert
            Assert.IsTrue(res.GetType().Name.StartsWith("Ok"));
        }

        [TestMethod]
        public void Trying_Login_Wrong_Role_Should_Return_Unauthorized()
        {
            // Arrange
            var req = new _250828_universityTask.Models.Requests.LoginRequest(1, "e@e.com", "test", "university");

            // Act + Assert
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
            {
                AuthEndpoints.LoginLogic(req, _identityService, _cacheService, _fileLoggerProvider);
            });
        }

        [TestMethod]
        public void Trying_Login_Wrong_Password_Should_Return_Unauthorized()
        {
            // Arrange
            var req = new _250828_universityTask.Models.Requests.LoginRequest(1, "e@e.com", "nothing", "professor");

            // Act + Assert
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
            {
                AuthEndpoints.LoginLogic(req, _identityService, _cacheService, _fileLoggerProvider);
            });
        }

        [TestMethod]
        public void Trying_Login_No_User_With_Id_Should_Return_Unauthorized()
        {
            // Arrange
            var req = new _250828_universityTask.Models.Requests.LoginRequest(0, "e@e.com", "test", "professor");

            // Act + Assert
            Assert.ThrowsException<UnauthorizedAccessException>(() =>
            {
                AuthEndpoints.LoginLogic(req, _identityService, _cacheService, _fileLoggerProvider);
            });
        }

        [TestMethod]
        public void Should_Return_False_For_Wrong_Password()
        {
            // Arrange - Act
            var(uniId, verified) = AuthEndpoints.VerifyPassword("nothing", 1, "professor", _cacheService);

            // Assert
            Assert.IsTrue(!verified);
        }

        [TestMethod]
        public void Should_Return_True_For_True_Password()
        {
            // Arrange - Act
            var (uniId, verified) = AuthEndpoints.VerifyPassword("test", 1, "professor", _cacheService);

            // Assert
            Assert.IsTrue(verified);
        }

        [TestMethod]
        public void Should_Return_False_For_Wrong_Role()
        {
            // Arrange - Act
            var (uniId, verified) = AuthEndpoints.VerifyPassword("test", 1, "university", _cacheService);

            // Assert
            Assert.IsTrue(!verified);
        }
    }
}
