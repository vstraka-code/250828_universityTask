using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Features.Students;
using _250828_universityTask.Logger;
using _250828_universityTask.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace _250828_universityTaskTests.Features.Students
{
    [TestClass]
    public class GetStudentHandlerTests
    {

        private CancellationToken cancellationToken;
        private CacheServiceWithoutExtension _cacheService;
        private GetStudentHandler _handler;
        private FileLoggerProvider _fileLoggerProvider;

        [TestInitialize]
        public void Setup()
        {
            cancellationToken = new CancellationToken();

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
                new Student { Id = 1, Name = "Sophia", UniversityId = 1 },
                new Student { Id = 3, Name = "Tom", UniversityId = 2 }
            }
            };

            var cache = new Cache(_fileLoggerProvider);
            var logger = Substitute.For<ILogger<CacheServiceWithoutExtension>>();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheService = new CacheServiceWithoutExtension(jsonDb, cache, logger, _fileLoggerProvider);
            _handler = new GetStudentHandler(_cacheService);
        }

        [TestMethod]
        public async Task Should_Return_Sophia_Student()
        {
            // Arrange
            var req = new GetStudentQuery(1, 1);

            // Act
            var res = await _handler.Handle(req, cancellationToken);

            // Assert
            Assert.AreEqual("Sophia", res.Name);
        }

        [TestMethod]
        public async Task Student_Not_Exist_Should_Return_KeyNotFoundException()
        {
            // Arrange
            var req = new GetStudentQuery(2, 1);

            // Act + Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
            {
                await _handler.Handle(req, cancellationToken);
            });
        }

        [TestMethod]
        public async Task Professor_Not_Exist_Should_Return_UnauthorizedAccessException()
        {
            // Arrange
            var req = new GetStudentQuery(1, 2);

            // Act + Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () =>
            {
                await _handler.Handle(req, cancellationToken);
            });
        }

        [TestMethod]
        public async Task Different_University_Should_Return_UnauthorizedAccessException()
        {
            // Arrange
            var req = new GetStudentQuery(3, 1);

            // Act + Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () =>
            {
                await _handler.Handle(req, cancellationToken);
            });
        }
    }
}
