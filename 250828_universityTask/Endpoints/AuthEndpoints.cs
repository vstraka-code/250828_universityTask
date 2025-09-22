using _250828_universityTask.Auth;
using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Features.Professors;
using _250828_universityTask.Features.Students;
using _250828_universityTask.Helpers;
using _250828_universityTask.Logger;
using _250828_universityTask.Models.Dtos;
using _250828_universityTask.Models.Requests;
using _250828_universityTask.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;

[assembly: InternalsVisibleTo("250828_universityTaskTests")]
namespace _250828_universityTask.Endpoints
{
    public static class AuthEndpoints
    {
        private const string ProfessorRole = "professor";
        private const string StudentRole = "student";
        private static Boolean exisiting = false;

        private static string mess = "";
        private static LoggerTopics topic = LoggerTopics.AuthEndpoints;
        public static void MapAuthEndpoints(this WebApplication app)
        {
            var authGroup = app.MapGroup("/api/auth");

            authGroup.MapPost("/login", (LoginRequest req, IValidator<LoginRequest> validator, IdentityService identityService, CacheServiceWithoutExtension cacheService, FileLoggerProvider fileLoggerProvider) =>
            {
                var validationResult = validator.Validate(req);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );

                    throw new Exceptions.ValidationException(errors);
                }

                return LoginLogic(req, identityService, cacheService, fileLoggerProvider);
            });

            authGroup.MapPost("/register", (RegistrationRequest req, IValidator<RegistrationRequest> validator, IMediator mediator, FileLoggerProvider fileLoggerProvider) =>
            {
                var validationResult = validator.Validate(req);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );

                    throw new Exceptions.ValidationException(errors);
                }

                return RegisterLogic(req, mediator, fileLoggerProvider);
            });
        }

        public static IResult LoginLogic(LoginRequest req, IdentityService identityService, CacheServiceWithoutExtension cacheService, FileLoggerProvider fileLoggerProvider)
        {

            if (req.Id is null)
            {
                mess = "No ID provided in login request.";
                fileLoggerProvider.SaveBehaviourLogging(mess, topic);
                throw new UnauthorizedAccessException();
            }

            var role = req.Role;
            var (uniId, verified) = VerifyPassword(req.Password, req.Id.Value, role, cacheService);

            if (!exisiting)
            {
                mess = "ID: " + req.Id + " not exisiting. Invalid ID.";
                fileLoggerProvider.SaveBehaviourLogging(mess, topic);

                throw new UnauthorizedAccessException();

            } else if (!verified)
            {
                if (role != ProfessorRole && role != StudentRole)
                {
                    mess = "Tried login in with undefined role: " + role + ", with id " + req.Id;
                    fileLoggerProvider.SaveBehaviourLogging(mess, topic);
                } else
                {
                    mess = role + " with id " + req.Id + " tried login in with wrong password.";
                    fileLoggerProvider.SaveBehaviourLogging(mess, topic);
                }

                throw new UnauthorizedAccessException();
            }
            
            if (role == ProfessorRole || role == StudentRole)
            {
                var token = identityService.CreateToken(req.Id.Value, role, uniId);

                mess = "Login as " + role + " successful with id " + req.Id;
                fileLoggerProvider.SaveBehaviourLogging(mess, topic);

                return Results.Ok(new AuthResponse(token));
            }

            throw new UnauthorizedAccessException();
        }

        public static async Task<IResult> RegisterLogic(RegistrationRequest req, IMediator mediator, FileLoggerProvider fileLoggerProvider)
        {
            if (req.UniId is null)
            {
                mess = "No Uni ID provided in registration request.";
                fileLoggerProvider.SaveBehaviourLogging(mess, topic);
                throw new UnauthorizedAccessException();
            }

            var professorName = req.Name;
            var professorUniId = req.UniId.Value;

            var professor = await mediator.Send(new AddProfessorCommand(professorName, professorUniId));

            mess = "Registration as professor successful with id " + professor.Id;
            fileLoggerProvider.SaveBehaviourLogging(mess, topic);

            return Results.Created($"/api/auth/register/{professor.Id}", professor);
        }

        internal static (int? uniId, bool verified) VerifyPassword(string password, int id, string role, CacheServiceWithoutExtension cacheService)
        {
            if (role == ProfessorRole)
            {
                var professors = cacheService.AllProfessors();
                // var professors = await cacheService.AllProfessors();
                var prof = professors.FirstOrDefault(p => p.Id == id);
                if (prof != null) exisiting = true;

                if (prof == null || password != "test")
                {
                    return (null, false);
                } else
                {
                    return (prof.UniversityId, true);
                }
            } else if (role == StudentRole)
            {
                var students = cacheService.AllStudents();
                // var students = await cacheService.AllStudents();
                var stud = students.FirstOrDefault(s => s.Id == id);
                if (stud != null) exisiting = true;

                if (stud == null || password != "test")
                {
                    return (null, false);
                }
                else
                {
                    return (stud.UniversityId, true);
                }
            }

            return (null, false);
        }
    }
}
