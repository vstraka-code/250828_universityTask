using _250828_universityTask.Data;
using _250828_universityTask.Features.Students;
using _250828_universityTask.Helpers;
using _250828_universityTask.Models;
using _250828_universityTask.Models.Dtos;
using MediatR;
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
                var professorId = user.GetProfessorIdOrThrow();
                var students = await mediator.Send(new GetAllStudentsQuery(professorId));
                return Results.Ok(students);
            });

            // get specific Student with the id
            studentsGroup.MapGet("/{id:int}", async (int id, ClaimsPrincipal user, IMediator mediator) =>
            {
                var professorId = user.GetProfessorIdOrThrow();
                var student = await mediator.Send(new GetStudentQuery(id, professorId));
                return Results.Ok(student);
            });

            // get info about yourself as student
            studentsGroup.MapGet("/me", async (ClaimsPrincipal user, IMediator mediator) =>
            {
                var studentId = user.GetStudentIdOrThrow();
                var student = await mediator.Send(new GetStudentQuery(studentId, null, studentId));
                return Results.Ok(student);
            });

            // Prof can add new student
            studentsGroup.MapPost("", async (CreateStudentRequest req, ClaimsPrincipal user, IMediator mediator) =>
            {
                var professorId = user.GetProfessorIdOrThrow();
                var student = await mediator.Send(new AddStudentCommand(req.Name, professorId));
                return Results.Created($"/api/students/{student.Id}", student);
                
            });

            // prof can edit student
            studentsGroup.MapPut("/{id:int}", async (int id, UpdateStudentRequest request, ClaimsPrincipal user, IMediator mediator) =>
            {
                var professorId = user.GetProfessorIdOrThrow();
                var student = await mediator.Send(new UpdateStudentCommand(id, request.Name, professorId));
                return Results.Ok(student);
            });

            // prof can delete student
            studentsGroup.MapDelete("/{id:int}", async (int id, ClaimsPrincipal user, IMediator mediator) =>
            {
                var professorId = user.GetProfessorIdOrThrow();
                var deleted = await mediator.Send(new DeleteStudentCommand(id, professorId));
                if (!deleted) return Results.NotFound();
                return Results.NoContent();
            });
        }
    }
}
