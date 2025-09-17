using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Exceptions;
using _250828_universityTask.Features.Students;
using _250828_universityTask.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace _250828_universityTaskTests.Features.Students
{
    [TestClass]
    public class AddStudentHandlerTests
    {
        private CancellationToken cancellationToken;
        private CacheService _cacheService;
        private AddStudentHandler _handler;

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

            var logger = Substitute.For<ILogger<CacheService>>();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheService = new CacheService(jsonDb, memoryCache, logger);
            _handler = new AddStudentHandler(_cacheService, jsonDb);
        }

        [TestMethod]
        public async Task Should_Return_Student()
        {
            // Arrange
            var req = new AddStudentCommand("Clara", 1);

            // Act
            var res = await _handler.Handle(req, cancellationToken);

            // Assert
            Assert.AreEqual("Clara", res.Name);
        }

        [TestMethod]
        public async Task Should_Return_ValidationException_If_Studentname_Already_Existing()
        {
            // Arrange
            var req = new AddStudentCommand("Sophia", 1);

            // Act + Assert

            await Assert.ThrowsExceptionAsync<ValidationException>(async () =>
            {
                await _handler.Handle(req, cancellationToken);
            });
        }

        [TestMethod]
        public async Task Professor_Not_Exist_Should_Return_UnauthorizedAccessException()
        {
            // Arrange
            var req = new AddStudentCommand("Clara", 2);

            // Act + Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () =>
            {
                await _handler.Handle(req, cancellationToken);
            });
        }
    }
}
