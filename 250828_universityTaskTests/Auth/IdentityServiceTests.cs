using _250828_universityTask.Auth;
using _250828_universityTask.Endpoints;
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

        [TestMethod]
        public void Should_Return_JWT_Token()
        {
            // Arrange - Act
            var token = IdentityService.CreateToken(_identityService, 1, "professor", 1);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(token));
        }
    }
}
