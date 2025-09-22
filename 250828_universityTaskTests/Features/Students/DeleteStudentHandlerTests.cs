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
    public class DeleteStudentHandlerTests
    {
        private CancellationToken cancellationToken;
        private CacheServiceWithoutExtension _cacheService;
        private DeleteStudentHandler _handler;
        private readonly Cache _cache;

        [TestInitialize]
        public void Setup()
        {
            cancellationToken = new CancellationToken();

            var uni1 = new University { Id = 1, Name = "Uni Vie", City = "Vienna", Country = "Austria" };
            var uni2 = new University { Id = 2, Name = "Uni Graz", City = "Graz", Country = "Austria" };

            var jsonDb = Substitute.For<IJsonDbContext>();
            jsonDb.Professors = new List<Professor>
            {
                new Professor { Id = 1, Name = "Klaus", UniversityId = 1, University = uni1},
                new Professor { Id = 3, Name = "Ulrike", UniversityId = 2, University = uni2 }
            };
            jsonDb.Students = new List<Student>
            {
                new Student { Id = 1, Name = "Sophia", UniversityId = 1 },
                new Student { Id = 2, Name = "Sarah", UniversityId = 1 }
            };
            jsonDb.When(x => x.Save()).Do(_ => { });

            var logger = Substitute.For<ILogger<CacheServiceWithoutExtension>>();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheService = new CacheServiceWithoutExtension(jsonDb, _cache, logger);
            _handler = new DeleteStudentHandler(jsonDb, _cacheService);
        }

        [TestMethod]
        public async Task Deleting_Student_Should_Return_True()
        {
            // Arrange
            var req = new DeleteStudentCommand(1, 1);

            // Act
            var res = await _handler.Handle(req, cancellationToken);

            // Assert
            Assert.IsTrue(res);
        }

        [TestMethod]
        public async Task Deleting_Not_Exisiting_Student_Should_Return_False()
        {
            // Arrange
            var req = new DeleteStudentCommand(3, 1);

            // Act
            var res = await _handler.Handle(req, cancellationToken);

            // Assert
            Assert.IsTrue(!res);
        }

        [TestMethod]
        public async Task Deleting_Student_As_Not_Exisiting_Professor_Should_Return_UnauthorizedAccessException()
        {
            // Arrange
            var req = new DeleteStudentCommand(1, 2);

            // Act + Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () =>
            {
                await _handler.Handle(req, cancellationToken);
            });
        }

        [TestMethod]
        public async Task Deleting_Student_As_Professor_From_Different_Uni_Should_Return_UnauthorizedAccessException()
        {
            // Arrange
            var req = new DeleteStudentCommand(1, 3);

            // Act + Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () =>
            {
                await _handler.Handle(req, cancellationToken);
            });
        }
    }
}
