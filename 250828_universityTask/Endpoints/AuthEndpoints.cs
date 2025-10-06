using _250828_universityTask.Auth;
using _250828_universityTask.Cache;
using _250828_universityTask.Features.Professors;
using _250828_universityTask.Helpers;
using _250828_universityTask.Logger;
using _250828_universityTask.Models.Dtos;
using _250828_universityTask.Models.Requests;
using FluentValidation;
using MediatR;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("250828_universityTaskTests")]
namespace _250828_universityTask.Endpoints
{
    public static class AuthEndpoints
    {
        #region Properties
        private static Boolean exisiting = false;
        private static string mess = "";
        [Inject] private static LoggerTopics _topic = LoggerTopics.AuthEndpoints;

        private const string PROFESSOR = "professor";
        private const string STUDENT = "student";
        #endregion

        public static void MapAuthEndpoints(this WebApplication app)
        {
            var authGroup = app.MapGroup("/api/auth");

            authGroup.MapPost("/login", (HttpContext context, /* LoginRequest req,*/ IValidator<LoginRequest> validator, IdentityService identityService, CacheServiceWithoutExtension cacheService, FileLoggerProvider fileLoggerProvider) =>
            {
                var req = GetHeaderValueExtension.GetHeaderValueLogin(context);
                ValidatorExtensions.ValidateResult(req, validator);
                return LoginLogic(req, identityService, cacheService, fileLoggerProvider);
            });

            authGroup.MapPost("/register", (HttpContext context, /* RegistrationRequest req, */ IValidator<RegistrationRequest> validator, IMediator mediator, FileLoggerProvider fileLoggerProvider) =>
            {
                var req = GetHeaderValueExtension.GetHeaderValueRegister(context);
                ValidatorExtensions.ValidateResult(req, validator);
                return RegisterLogic(req, mediator, fileLoggerProvider);
            });
        }

        #region Logic
        internal static IResult LoginLogic(LoginRequest req, IdentityService identityService, CacheServiceWithoutExtension cacheService, FileLoggerProvider fileLoggerProvider)
        {

            /* 
                cases
                0 as ID: No ID provided in login request

                first if : Tried login in with undefined role:
                invalid role + invalid password + invalid id
                invalid role + invalid password + id
                invalid role + password + invalid id
                invalid role + password + id

                second if : ID: " + req.Id + " not exisiting. Invalid ID
                role + password + invalid id
                role + invalid password + invalid id

                third if: with id " + req.Id + " tried login in with wrong password
                role + invalid password + id 
            */

            if (req.Id is null || req.Id == 0) // mainly so I can use req.Id.Value without issues
            {
                mess = "No ID provided in login request.";
                fileLoggerProvider.SaveBehaviourLogging(mess, _topic);
                throw new UnauthorizedAccessException();
            }

            var role = req.Role;
            var (uniId, verified) = VerifyPassword(req.Password, req.Id.Value, role, cacheService);

            if (role != PROFESSOR && role != STUDENT)
            {
                mess = "Tried login in with undefined role: " + role + ", with id " + req.Id;
                fileLoggerProvider.SaveBehaviourLogging(mess, _topic);
                throw new UnauthorizedAccessException();
            } else if (!exisiting)
            {
                mess = "ID: " + req.Id + " not exisiting. Invalid ID.";
                fileLoggerProvider.SaveBehaviourLogging(mess, _topic);
                throw new UnauthorizedAccessException();
            }
            else if (!verified)
            {
                mess = role + " with id " + req.Id + " tried login in with wrong password.";
                fileLoggerProvider.SaveBehaviourLogging(mess, _topic);
                throw new UnauthorizedAccessException();
            }
            
            if (role == PROFESSOR || role == STUDENT && verified && exisiting)
            {
                var token = identityService.CreateToken(req.Id.Value, role, uniId);

                mess = "Login as " + role + " successful with id " + req.Id;
                fileLoggerProvider.SaveBehaviourLogging(mess, _topic);

                return Results.Ok(new AuthResponse(token));
            }

            throw new UnauthorizedAccessException();
        }

        private static async Task<IResult> RegisterLogic(RegistrationRequest req, IMediator mediator, FileLoggerProvider fileLoggerProvider)
        {
            if (req.UniId is null) // mainly so I can use req.UniId.Value without issues
            {
                mess = "No Uni ID provided in registration request.";
                fileLoggerProvider.SaveBehaviourLogging(mess, _topic);
                throw new UnauthorizedAccessException();
            }

            var professorName = req.Name;
            var professorUniId = req.UniId.Value;

            var professor = await mediator.Send(new AddProfessorCommand(professorName, professorUniId));

            mess = "Registration as professor successful with id " + professor.Id;
            fileLoggerProvider.SaveBehaviourLogging(mess, _topic);

            return Results.Created($"/api/auth/register/{professor.Id}", professor);
        }
        #endregion

        internal static (int? uniId, bool verified) VerifyPassword(string password, int id, string role, CacheServiceWithoutExtension cacheService)
        {
            if (role == PROFESSOR)
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
            } else if (role == STUDENT)
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
