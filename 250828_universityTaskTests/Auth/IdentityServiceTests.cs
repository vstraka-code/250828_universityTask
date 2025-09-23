using _250828_universityTask.Auth;
using _250828_universityTask.Endpoints;
using _250828_universityTask.Logger;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _250828_universityTaskTests.Auth
{
    [TestClass]
    public class IdentityServiceTests
    {
        private IdentityService _identityService;
        private FileLoggerProvider _fileLoggerProvider;

        [TestInitialize]
        public void Setup()
        {
            var logger = Substitute.For<ILogger<FileLoggerProvider>>();
            _fileLoggerProvider = new FileLoggerProvider(logger, disableFileIO: true);

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
        public void Should_Return_JWT_Token()
        {
            // Arrange - Act
            var token = _identityService.CreateToken(1, "professor", 1);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(token));
        }
    }
}
