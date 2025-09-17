using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Exceptions;
using _250828_universityTask.Features.Students;
using _250828_universityTask.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _250828_universityTaskTests.Features.Students
{
    [TestClass]
    public class UpdateStudentHandlerTests
    {
        private CancellationToken cancellationToken;
        private CacheService _cacheService;
        private UpdateStudentHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            cancellationToken = new CancellationToken();

            var jsonDb = Substitute.For<IJsonDbContext>();
            jsonDb.Professors = new List<Professor>
            {
                new Professor { Id = 1, Name = "Klaus", UniversityId = 1 },
                new Professor { Id = 3, Name = "Ulrike", UniversityId = 2 }
            };
            jsonDb.Students = new List<Student>
            {
                new Student { Id = 1, Name = "Sophia", UniversityId = 1 },
                new Student { Id = 2, Name = "Sarah", UniversityId = 1 }
            };
            jsonDb.When(x => x.Save()).Do(_ => {});

            var logger = Substitute.For<ILogger<CacheService>>();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheService = new CacheService(jsonDb, memoryCache, logger);
            _handler = new UpdateStudentHandler(jsonDb, _cacheService);
        }

        [TestMethod]
        public async Task Should_Return_Student_With_New_Name()
        {
            // Arrange
            var req = new UpdateStudentCommand(1, "Clara", 1);

            // Act
            var res = await _handler.Handle(req, cancellationToken);

            // Assert
            Assert.AreEqual("Clara", res.Name);
        }

        [TestMethod]
        public async Task Should_Return_Student_With_No_Change_If_Name_Stays_The_Same()
        {
            // Arrange
            var req = new UpdateStudentCommand(1, "Sophia", 1);

            // Act
            var res = await _handler.Handle(req, cancellationToken);

            // Assert
            Assert.AreEqual("Sophia", res.Name);
        }

        [TestMethod]
        public async Task Should_Return_Throw_ValidationException_If_Name_Already_Exists()
        {
            // Arrange
            var req = new UpdateStudentCommand(1, "Sarah", 1);

            // Act + Assert
            await Assert.ThrowsExceptionAsync<ValidationException>(async () =>
            {
                await _handler.Handle(req, cancellationToken);
            });
        }

        [TestMethod]
        public async Task Student_Not_Exist_Should_Return_KeyNotFoundException()
        {
            // Arrange
            var req = new UpdateStudentCommand(3, "Tom", 1);

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
            var req = new UpdateStudentCommand(1, "Tom", 2);

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
            var req = new UpdateStudentCommand(1, "Herbert", 3);

            // Act + Assert
            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () =>
            {
                await _handler.Handle(req, cancellationToken);
            });
        }
    }
}
