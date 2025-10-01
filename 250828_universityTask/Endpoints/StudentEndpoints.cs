using _250828_universityTask.Cache;
using _250828_universityTask.Features.Students;
using _250828_universityTask.Helpers;
using _250828_universityTask.Logger;
using _250828_universityTask.Models.Requests;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace _250828_universityTask.Endpoints
{
    public static class StudentEndpoints
    {
        private static string mess = "";
        private static LoggerTopics topic = LoggerTopics.StudentEndpoints;
        private const string PROFESSOR = "professor";
        private const string STUDENT = "student";
        public static void MapStudentEndpoints(this WebApplication app)
        {
            var studentsGroup = app.MapGroup("/api/students");

            // GET All Students from the uni that the prof belongs to
            studentsGroup.MapGet("", async (ClaimsPrincipal user, IMediator mediator, FileLoggerProvider fileLoggerProvider) =>
            {
                return await GetAllStudentsLogic(user, mediator, fileLoggerProvider);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = PROFESSOR });

            // get specific Student with the id
            studentsGroup.MapGet("/{id:int}", async (int id, ClaimsPrincipal user, IMediator mediator, FileLoggerProvider fileLoggerProvider) =>
            {
                return await GetStudentLogic(id, user, mediator, fileLoggerProvider);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = PROFESSOR });

            // get info about yourself as student
            studentsGroup.MapGet("/me", async (ClaimsPrincipal user, IMediator mediator, FileLoggerProvider fileLoggerProvider) =>
            {
                return await GetStudentAsStudentLogic(user, mediator, fileLoggerProvider);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = STUDENT });

            // Prof can add new student
            studentsGroup.MapPost("", async (HttpContext context, /* AddStudentRequest req, */ IValidator <AddStudentRequest> validator, ClaimsPrincipal user, IMediator mediator, FileLoggerProvider fileLoggerProvider) =>
            {
                var req = GetHeaderValueExtension.GetHeaderValueAdd(context);
                ValidatorExtensions.ValidateResult(req, validator);
                return await AddStudentLogic(req, user, mediator, fileLoggerProvider);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = PROFESSOR });

            // prof can edit student
            studentsGroup.MapPut("/{id:int}", async (int id, HttpContext context, /* UpdateStudentRequest req, */ IValidator < UpdateStudentRequest > validator, ClaimsPrincipal user, IMediator mediator, FileLoggerProvider fileLoggerProvider) =>
            {
                var req = GetHeaderValueExtension.GetHeaderValueUpdate(context);
                ValidatorExtensions.ValidateResult(req, validator);
                return await UpdateStudentLogic(id, req, user, mediator, fileLoggerProvider);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = PROFESSOR });

            // prof can delete student
            studentsGroup.MapDelete("/{id:int}", async (int id, ClaimsPrincipal user, IMediator mediator, FileLoggerProvider fileLoggerProvider) =>
            {
                return await DeleteStudentLogic(id, user, mediator, fileLoggerProvider);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = PROFESSOR });

            // prof can clear cache
            studentsGroup.MapPost("/clear-cache", (CacheServiceWithoutExtension _cacheService) =>
            {
                return _cacheService.ClearCache();
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = PROFESSOR });
        }

        public static async Task<IResult> AddStudentLogic(AddStudentRequest req, ClaimsPrincipal user, IMediator mediator, FileLoggerProvider fileLoggerProvider)
        {
            var professorId = user.GetProfessorId();
            var student = await mediator.Send(new AddStudentCommand(req.Name, professorId));

            mess = "Professor with id " + professorId + " added student with id " + student.Id;
            fileLoggerProvider.SaveBehaviourLogging(mess, topic);

            return Results.Created($"/api/students/{student.Id}", student);
        }

        public static async Task<IResult> DeleteStudentLogic(int id, ClaimsPrincipal user, IMediator mediator, FileLoggerProvider fileLoggerProvider)
        {
            var professorId = user.GetProfessorId();
            var deleted = await mediator.Send(new DeleteStudentCommand(id, professorId));
            if (!deleted) return Results.NotFound();

            mess = "Professor with id " + professorId + " deleted student with id " + id;
            fileLoggerProvider.SaveBehaviourLogging(mess, topic);

            return Results.NoContent();
        }

        public static async Task<IResult> UpdateStudentLogic(int id, UpdateStudentRequest request, ClaimsPrincipal user, IMediator mediator, FileLoggerProvider fileLoggerProvider)
        {
            var professorId = user.GetProfessorId();
            var student = await mediator.Send(new UpdateStudentCommand(id, request.Name, professorId));

            mess = "Professor with id " + professorId + " updated student with id " + id;
            fileLoggerProvider.SaveBehaviourLogging(mess, topic);

            return Results.Ok(student);
        }

        public static async Task<IResult> GetAllStudentsLogic(ClaimsPrincipal user, IMediator mediator, FileLoggerProvider fileLoggerProvider)
        {
            var professorId = user.GetProfessorId();
            var students = await mediator.Send(new GetAllStudentsQuery(professorId));

            mess = "Professor with id " + professorId + " received all Students";
            fileLoggerProvider.SaveBehaviourLogging(mess, topic);

            return Results.Ok(students);
        }

        public static async Task<IResult> GetStudentLogic(int id, ClaimsPrincipal user, IMediator mediator, FileLoggerProvider fileLoggerProvider)
        {
            var professorId = user.GetProfessorId();
            var student = await mediator.Send(new GetStudentQuery(id, professorId));

            mess = "Professor with id " + professorId + " received Student with id " + id;
            fileLoggerProvider.SaveBehaviourLogging(mess, topic);

            return Results.Ok(student);
        }

        public static async Task<IResult> GetStudentAsStudentLogic(ClaimsPrincipal user, IMediator mediator, FileLoggerProvider fileLoggerProvider)
        {
            var studentId = user.GetStudentId();
            var student = await mediator.Send(new GetStudentQuery(studentId, null, studentId));

            mess = "Student itself received Studeninformation as Student with id " + studentId;
            fileLoggerProvider.SaveBehaviourLogging(mess, topic);

            return Results.Ok(student);
        }
    }
}
