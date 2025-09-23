using _250828_universityTask.Auth;
using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Endpoints;
using _250828_universityTask.Features.Students;
using _250828_universityTask.Helpers;
using _250828_universityTask.Logger;
using _250828_universityTask.Models;
using _250828_universityTask.Models.Dtos;
using _250828_universityTask.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace _250828_universityTaskTests.Endpoints
{
    [TestClass]
    public class StudentEndpointsTests
    {
        private ServiceCollection _services;
        private FileLoggerProvider _fileLoggerProvider;


        [TestInitialize]
        public void Setup()
        {
            var logger = Substitute.For<ILogger<FileLoggerProvider>>();
            _fileLoggerProvider = new FileLoggerProvider(logger, disableFileIO: true);

            _services = new ServiceCollection();
            _services.AddLogging();
        }

        [TestMethod]
        public async Task Adding_New_Student_Should_Return_Created()
        {
            // Arrange
            var req = new AddStudentRequest("Sophia");

            var claims = new[] { new Claim("ProfessorId", "1") };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            // always return this so the real DB/Cache Service doesn't get involved
            var mediator = Substitute.For<IMediator>();
            mediator.Send(Arg.Any<AddStudentCommand>())
                    .Returns(new StudentDto (3, "Sophia", "Uni Vie", "Klaus"));

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = _services.BuildServiceProvider();

            // Act
            var res = await StudentEndpoints.AddStudentLogic(req, user, mediator, _fileLoggerProvider);
            await res.ExecuteAsync(httpContext);

            // Assert
            Assert.AreEqual(StatusCodes.Status201Created, httpContext.Response.StatusCode);
        }

        [TestMethod]
        public async Task Deleting_Student_Should_Return_NoContent()
        {
            // Arrange
            var claims = new[] { new Claim("ProfessorId", "1") };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            var claim = user.FindFirst("ProfessorId");
            int id = int.Parse(claim.Value);

            // always return this so the real DB/Cache Service doesn't get involved
            var mediator = Substitute.For<IMediator>();
            mediator.Send(Arg.Any<DeleteStudentCommand>())
                    .Returns(true);

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = _services.BuildServiceProvider();

            // Act
            var res = await StudentEndpoints.DeleteStudentLogic(id, user, mediator, _fileLoggerProvider);
            await res.ExecuteAsync(httpContext);

            // Assert
            Assert.AreEqual(StatusCodes.Status204NoContent, httpContext.Response.StatusCode);
        }

        [TestMethod]
        public async Task Updating_Student_Should_Return_Ok()
        {
            // Arrange
            var req = new UpdateStudentRequest("Mira");

            var claims = new[] { new Claim("ProfessorId", "1") };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            var claim = user.FindFirst("ProfessorId");
            int id = int.Parse(claim.Value);

            // always return this so the real DB/Cache Service doesn't get involved
            var mediator = Substitute.For<IMediator>();
            mediator.Send(Arg.Any<UpdateStudentCommand>())
                    .Returns(new StudentDto(3, "Sophia", "Uni Vie", "Klaus"));

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = _services.BuildServiceProvider();

            // Act
            var res = await StudentEndpoints.UpdateStudentLogic(id, req, user, mediator, _fileLoggerProvider);
            await res.ExecuteAsync(httpContext);

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        }

        [TestMethod]
        public async Task Get_All_Students_Should_Return_Ok()
        {
            // Arrange
            var claims = new[] { new Claim("ProfessorId", "1") };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            // always return this so the real DB/Cache Service doesn't get involved
            var mediator = Substitute.For<IMediator>();
            mediator.Send(Arg.Any<GetAllStudentsQuery>())
                    .Returns(new List<StudentDto> 
                    {
                        new StudentDto(1, "Tom", "Uni Vie", "Klaus"),
                        new StudentDto(2, "Jerry", "TU Graz", "Julia")
                    });

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = _services.BuildServiceProvider();

            // Act
            var res = await StudentEndpoints.GetAllStudentsLogic(user, mediator, _fileLoggerProvider);
            await res.ExecuteAsync(httpContext);

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        }

        [TestMethod]
        public async Task Get_Student_Should_Return_Ok()
        {
            // Arrange
            var claims = new[] { new Claim("ProfessorId", "1") };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            var claim = user.FindFirst("ProfessorId");
            int id = int.Parse(claim.Value);

            // always return this so the real DB/Cache Service doesn't get involved
            var mediator = Substitute.For<IMediator>();
            mediator.Send(Arg.Any<GetStudentQuery>())
                    .Returns(new StudentDto(3, "Sophia", "Uni Vie", "Klaus"));

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = _services.BuildServiceProvider();

            // Act
            var res = await StudentEndpoints.GetStudentLogic(id, user, mediator, _fileLoggerProvider);
            await res.ExecuteAsync(httpContext);

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        }

        [TestMethod]
        public async Task Get_Student_As_Student_Should_Return_Ok()
        {
            // Arrange
            var claims = new[] { new Claim("StudentId", "1") };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"));

            // always return this so the real DB/Cache Service doesn't get involved
            var mediator = Substitute.For<IMediator>();
            mediator.Send(Arg.Any<GetStudentQuery>())
                    .Returns(new StudentDto(1, "Tom", "Uni Vie", "Klaus"));

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = _services.BuildServiceProvider();

            // Act
            var res = await StudentEndpoints.GetStudentAsStudentLogic(user, mediator, _fileLoggerProvider);
            await res.ExecuteAsync(httpContext);

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        }
    }
}
