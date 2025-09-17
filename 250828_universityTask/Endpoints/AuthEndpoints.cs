using _250828_universityTask.Auth;
using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Features.Professors;
using _250828_universityTask.Features.Students;
using _250828_universityTask.Helpers;
using _250828_universityTask.Models.Dtos;
using _250828_universityTask.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
        public static void MapAuthEndpoints(this WebApplication app)
        {
            var authGroup = app.MapGroup("/api/auth");

            authGroup.MapPost("/login", (LoginRequest req, IdentityService identityService, CacheServiceWithoutExtension cacheService) =>
            {
                return LoginLogic(req, identityService, cacheService);
            });

            authGroup.MapPost("/register", (RegistrationRequest req, IMediator mediator) =>
            {
                return RegisterLogic(req, mediator);
            });
        }

        public static IResult LoginLogic(LoginRequest req, IdentityService identityService, CacheServiceWithoutExtension cacheService)
        {
            var role = req.Role;
            var (uniId, verified) = VerifyPassword(req.Password, req.Id, role, cacheService);

            if (!verified) return Results.Unauthorized();

            if (role == ProfessorRole || role == StudentRole)
            {
                var token = CreateToken(identityService, req.Id, role, uniId);
                return Results.Ok(new AuthResponse(token));
            }

            return Results.Unauthorized();
        }

        public static async Task<IResult> RegisterLogic(RegistrationRequest req, IMediator mediator)
        {
            var professorName = req.Name;
            var professorUniId = req.UniId;

            var professor = await mediator.Send(new AddProfessorCommand(professorName, professorUniId));

            return Results.Created($"/api/auth/register/{professor.Id}", professor);
        }

        internal static string CreateToken(IdentityService identityService, int id, string role, int? uniId = null)
        {
            var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, id.ToString()), //subject
                        new Claim(ClaimTypes.Role, role),
                    };
            if (role == ProfessorRole)
            {
                claims.Add(new("ProfessorId", id.ToString()));
                claims.Add(new("UniversityId", uniId?.ToString() ?? "")); // if id !null convert to string, otherwise empty string
            } else if (role == StudentRole)
            {
                claims.Add(new("StudentId", id.ToString()));
            }

            var token = identityService.WriteToken(identityService.CreateSecurityToken(new ClaimsIdentity(claims)));
            return token;
        }

        internal static (int? uniId, bool verified) VerifyPassword(string password, int id, string role, CacheServiceWithoutExtension cacheService)
        {
            if (role == ProfessorRole)
            {
                var professors = cacheService.AllProfessors();
                // var professors = await cacheService.AllProfessors();
                var prof = professors.FirstOrDefault(p => p.Id == id);

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
