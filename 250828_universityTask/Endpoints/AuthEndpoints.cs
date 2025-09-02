using _250828_universityTask.Auth;
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
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;

namespace _250828_universityTask.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            var authGroup = app.MapGroup("/api/auth");

            authGroup.MapPost("/login/professor", async (LoginProfessorRequest req, AppDbContext db, IdentityService identityService, IOptions<JwtSettings> opts) =>
            {
                var prof = await db.Professors.FindAsync(req.ProfessorId);
                // demo password check, replace with hashed verify in real app
                if (prof == null || req.Password != "test") return Results.Unauthorized();

                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, prof.Id.ToString()),
                new Claim(ClaimTypes.Role, "professor"),
                new Claim("ProfessorId", prof.Id.ToString()),
                new Claim("UniversityId", prof.UniversityId.ToString())
            };

                var token = identityService.WriteToken(identityService.CreateSecurityToken(new ClaimsIdentity(claims)));

                return Results.Ok(new AuthResponse(token));
            });

            authGroup.MapPost("/login/student", async (LoginStudentRequest req, AppDbContext db, IdentityService identityService) =>
            {
                var student = await db.Students.FindAsync(req.StudentId);
                if (student == null || req.Password != "test") return Results.Unauthorized();

                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, student.Id.ToString()),
                new Claim(ClaimTypes.Role, "student"),
                new Claim("StudentId", student.Id.ToString())
            };
                if (student.UniversityId.HasValue)
                    claims.Add(new Claim("UniversityId", student.UniversityId.Value.ToString()));

                var token = identityService.WriteToken(identityService.CreateSecurityToken(new ClaimsIdentity(claims)));
                return Results.Ok(new AuthResponse(token));
            });
        }
    }
}
