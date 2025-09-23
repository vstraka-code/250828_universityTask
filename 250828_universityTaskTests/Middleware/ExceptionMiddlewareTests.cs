using _250828_universityTask.Cache;
using _250828_universityTask.Logger;
using _250828_universityTask.Middleware;
using _250828_universityTask.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Text.Json;
using System.Threading.Tasks;

namespace _250828_universityTaskTests.Middleware
{
    [TestClass]
    public class ExceptionMiddlewareTests
    {
        private ILogger<ExceptionMiddleware> _logger;
        private FileLoggerProvider _fileLoggerProvider;

        [TestInitialize]
        public void Setup()
        {
            var logger = Substitute.For<ILogger<FileLoggerProvider>>();
            _fileLoggerProvider = new FileLoggerProvider(logger, disableFileIO: true);

            _logger = Substitute.For<ILogger<ExceptionMiddleware>>();
        }

        [TestMethod]
        public async Task Should_Return_403()
        {
            // Arrange
            var exception = new UnauthorizedAccessException();
            var wasExecuted = false;

            Task next(HttpContext context)
            {
                wasExecuted = true;
                throw exception;
            }

            var middleware = new ExceptionMiddleware(next, _logger, _fileLoggerProvider);
            var httpContext = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            Assert.IsTrue(wasExecuted);
            Assert.AreEqual(StatusCodes.Status403Forbidden, httpContext.Response.StatusCode);
        }

        [TestMethod]
        public async Task Should_Return_400()
        {
            // Arrange
            var exception = new ArgumentNullException();
            var wasExecuted = false;

            Task next(HttpContext context)
            {
                wasExecuted = true;
                throw exception;
            }

            var middleware = new ExceptionMiddleware(next, _logger, _fileLoggerProvider);
            var httpContext = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            Assert.IsTrue(wasExecuted);
            Assert.AreEqual(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
        }

        [TestMethod]
        public async Task Should_Write_Specific_Json_Response()
        {
            // Arrange
            var exception = new UnauthorizedAccessException("Forbidden");

            Task next(HttpContext context)
            {
                throw exception;
            }

            var middleware = new ExceptionMiddleware(next, _logger, _fileLoggerProvider);
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            httpContext.Response.Body.Position = 0;
            var body = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();

            var response = JsonSerializer.Deserialize<ErrorResponse>(body, new JsonSerializerOptions{
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(response);
            Assert.AreEqual(StatusCodes.Status403Forbidden, httpContext.Response.StatusCode);
            Assert.AreEqual("Forbidden", response.Message);
        }
    }
}
