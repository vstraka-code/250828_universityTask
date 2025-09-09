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

            authGroup.MapPost("/login", async (Models.Requests.LoginRequest req, [FromServices] JsonDbContext json, [FromServices] IdentityService identityService, [FromServices] CacheService cacheService) =>
            {
                var role = req.Role;

                if (role == "p")
                {
                    var professors = cacheService.AllProfessors();
                    // var professors = await cacheService.AllProfessors();

                    var prof = professors.FirstOrDefault(p => p.Id == req.Id);

                    if (prof == null || req.Password != "test")
                        return Results.Unauthorized();

                    var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, prof.Id.ToString()),
                        new Claim(ClaimTypes.Role, "professor"),
                        new Claim("ProfessorId", prof.Id.ToString()),
                        new Claim("UniversityId", prof.UniversityId.ToString())
                    };

                    var token = identityService.WriteToken(identityService.CreateSecurityToken(new ClaimsIdentity(claims)));
                    return Results.Ok(new AuthResponse(token));

                } else if (role == "s")
                {
                    var students = cacheService.AllStudents();
                    // var students = await cacheService.AllStudents();
                    var stud = students.FirstOrDefault(s => s.Id == req.Id);

                    if (stud == null || req.Password != "test")
                        return Results.Unauthorized();

                    var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, stud.Id.ToString()),
                        new Claim(ClaimTypes.Role, "student"),
                        new Claim("StudentId", stud.Id.ToString()),
                    };

                    var token = identityService.WriteToken(identityService.CreateSecurityToken(new ClaimsIdentity(claims)));
                    return Results.Ok(new AuthResponse(token));
                }

                return Results.Unauthorized();
            });
        }
    }
}
