using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Features.Students;
using _250828_universityTask.Helpers;
using _250828_universityTask.Models;
using _250828_universityTask.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;

namespace _250828_universityTask.Endpoints
{
    public static class StudentEndpoints
    {
        public static void MapStudentEndpoints(this WebApplication app)
        {
            var studentsGroup = app.MapGroup("/api/students");

            // GET All Students from the uni that the prof belongs to
            studentsGroup.MapGet("", async (ClaimsPrincipal user, IMediator mediator) =>
            {
                return await GetAllStudentsLogic(user, mediator);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "professor" });

            // get specific Student with the id
            studentsGroup.MapGet("/{id:int}", async (int id, ClaimsPrincipal user, IMediator mediator) =>
            {
                return await GetStudentLogic(id, user, mediator);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "professor" });

            // get info about yourself as student
            studentsGroup.MapGet("/me", async (ClaimsPrincipal user, IMediator mediator) =>
            {
                return await GetStudentAsStudentLogic(user, mediator);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "student" });

            // Prof can add new student
            studentsGroup.MapPost("", async (AddStudentRequest req, ClaimsPrincipal user, IMediator mediator) =>
            {
                return await AddStudentLogic(req, user, mediator);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "professor" });

            // prof can edit student
            studentsGroup.MapPut("/{id:int}", async (int id, UpdateStudentRequest request, ClaimsPrincipal user, IMediator mediator) =>
            {
                return await UpdateStudentLogic(id, request, user, mediator);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "professor" });

            // prof can delete student
            studentsGroup.MapDelete("/{id:int}", async (int id, ClaimsPrincipal user, IMediator mediator) =>
            {
                return await DeleteStudentLogic(id, user, mediator);
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "professor" });

            // prof can clear cache
            studentsGroup.MapPost("/clear-cache", (CacheService _cacheService) =>
            {
                return _cacheService.ClearCache();
            })
                .RequireAuthorization(new AuthorizeAttribute { Roles = "professor"});
        }

        public static async Task<IResult> AddStudentLogic(AddStudentRequest req, ClaimsPrincipal user, IMediator mediator)
        {
            var professorId = user.GetProfessorId();
            var student = await mediator.Send(new AddStudentCommand(req.Name, professorId));
            return Results.Created($"/api/students/{student.Id}", student);
        }

        public static async Task<IResult> DeleteStudentLogic(int id, ClaimsPrincipal user, IMediator mediator)
        {
            var professorId = user.GetProfessorId();
            var deleted = await mediator.Send(new DeleteStudentCommand(id, professorId));
            if (!deleted) return Results.NotFound();
            return Results.NoContent();
        }

        public static async Task<IResult> UpdateStudentLogic(int id, UpdateStudentRequest request, ClaimsPrincipal user, IMediator mediator)
        {
            var professorId = user.GetProfessorId();
            var student = await mediator.Send(new UpdateStudentCommand(id, request.Name, professorId));
            return Results.Ok(student);
        }

        public static async Task<IResult> GetAllStudentsLogic(ClaimsPrincipal user, IMediator mediator)
        {
            var professorId = user.GetProfessorId();
            var students = await mediator.Send(new GetAllStudentsQuery(professorId));
            return Results.Ok(students);
        }

        public static async Task<IResult> GetStudentLogic(int id, ClaimsPrincipal user, IMediator mediator)
        {
            var professorId = user.GetProfessorId();
            var student = await mediator.Send(new GetStudentQuery(id, professorId));
            return Results.Ok(student);
        }

        public static async Task<IResult> GetStudentAsStudentLogic(ClaimsPrincipal user, IMediator mediator)
        {
            var studentId = user.GetStudentId();
            var student = await mediator.Send(new GetStudentQuery(studentId, null, studentId));
            return Results.Ok(student);
        }
    }
}
