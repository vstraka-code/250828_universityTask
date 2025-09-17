using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Features.Students;
using _250828_universityTask.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace _250828_universityTaskTests.Features.Students
{
    [TestClass]
    public class GetAllStudentsHandlerTests
    {
        private CancellationToken cancellationToken;
        private CacheService _cacheService;
        private GetAllStudentsHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            cancellationToken = new CancellationToken();

            var jsonDb = new JsonDbContext
            {
                Professors = new List<Professor>
            {
                new Professor { Id = 1, Name = "Klaus", UniversityId = 1 },
                new Professor { Id = 2, Name = "Clara", UniversityId = 3 }
            },
                Students = new List<Student>
            {
                new Student { Id = 1, Name = "Sophia", UniversityId = 1 },
                new Student { Id = 2, Name = "Paul", UniversityId = 1 },
                new Student { Id = 3, Name = "Tom", UniversityId = 2 }
            }
            };

            var logger = Substitute.For<ILogger<CacheService>>();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheService = new CacheService(jsonDb, memoryCache, logger);
            _handler = new GetAllStudentsHandler(_cacheService);
        }

        [TestMethod]
        public async Task Should_Return_All_Students()
        {
            // Arrange
            var req = new GetAllStudentsQuery(1);

            // Act
            var res = await _handler.Handle(req, cancellationToken);

            // Assert
            Assert.AreEqual(2, res.Count);
            Assert.IsTrue(res.Any(s => s.Name == "Sophia"));
            Assert.IsTrue(res.Any(s => s.Name == "Paul"));
        }

        [TestMethod]
        public async Task Student_Not_Exist_Should_Return_EmptyList()
        {
            // Arrange
            var req = new GetAllStudentsQuery(2);

            // Act
            var res = await _handler.Handle(req, cancellationToken);

            // Assert
            Assert.AreEqual(0, res.Count);
        }

        [TestMethod]
        public async Task Professor_Not_Exist_Should_Return_UnauthorizedAccessException()
        {
            // Arrange
            var req = new GetAllStudentsQuery(3);

            // Act + Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () =>
            {
                await _handler.Handle(req, cancellationToken);
            });
        }
    }
}
