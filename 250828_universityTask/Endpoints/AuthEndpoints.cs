using _250828_universityTask.Auth;
using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Features.Students;
using _250828_universityTask.Helpers;
using _250828_universityTask.Models.Dtos;
using _250828_universityTask.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Security.Claims;

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

            authGroup.MapPost("/login", (Models.Requests.LoginRequest req, JsonDbContext json, IdentityService identityService, CacheService cacheService) =>
            {
                return LoginLogic(req, identityService, cacheService);
            });
        }

        public static IResult LoginLogic(Models.Requests.LoginRequest req, IdentityService identityService, CacheService cacheService)
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

        internal static (int? uniId, bool verified) VerifyPassword(string password, int id, string role, CacheService cacheService)
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
