using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace _250828_universityTask.Auth
{
    // JWT authentication middleware
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder RegisterAuthentication(this WebApplicationBuilder builder)
        {
            var jwtSection = builder.Configuration.GetSection(nameof(JwtSettings));
            builder.Services.Configure<JwtSettings>(jwtSection);
            var jwtSettings = jwtSection.Get<JwtSettings>()!;

            // IdentityService (token creation helper) as a singleton (created once)
            builder.Services.AddSingleton<IdentityService>();

            // sets JWT Bearer as default scheme
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                // configures how incoming JWT tokens are validated
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudiences = jwtSettings.Audiences,
                        RequireExpirationTime = true,
                        ValidateLifetime = true
                    };
                    options.Audience = jwtSettings.Audiences?.FirstOrDefault();
                    options.ClaimsIssuer = jwtSettings.Issuer;

                    // custom error res
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401; // Unauthorized
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsJsonAsync(new { error = "Invalid or missing token" });
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = 403; // Forbidden
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsJsonAsync(new { error = "You do not have permission to perform this action." });
                        }
                    };
                });

            builder.Services.AddAuthorization();

            // to use it in program.cs
            return builder;
        }
    }
}
