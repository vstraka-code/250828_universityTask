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
            // read configuration section (appsettings)
            var jwtConfig = builder.Configuration.GetSection(nameof(JwtSettings));
            // binds the settings
            builder.Services.Configure<JwtSettings>(jwtConfig);
            // binds values to the settings?
            var jwtSettings = jwtConfig.Get<JwtSettings>();

            if (jwtSettings == null)
            {
                throw new InvalidOperationException("JwtSettings config section is missing");
            }

            // registers IdentityService (token creation helper) as a singleton (created once)
            builder.Services.AddSingleton<IdentityService>();

            // sets JWT Bearer as default scheme
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //authenticating incoming requests
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // fallback
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // challenge unauthenticated requests
                })
                // configures how incoming JWT tokens are validated
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    // rules to validate the token
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudiences = jwtSettings.Audiences,
                        RequireExpirationTime = true,
                        ValidateLifetime = true
                    };
                    // if Audiences is !null, call .FirstOrDefault(), If null return null
                    options.Audience = jwtSettings.Audiences?.FirstOrDefault();

                    // if (jwtSettings.Audiences != null)
                    // {
                    //     options.Audience = jwtSettings.Audiences.FirstOrDefault();
                    // }
                    // else
                    // {
                    //     options.Audience = null;
                    // }

                    options.ClaimsIssuer = jwtSettings.Issuer;

                    // custom error res (needed here because it is before exceptionmiddleware gets used)
                    options.Events = new JwtBearerEvents
                    {
                        // Auth fails
                        OnChallenge = context =>
                        {
                            // prevents default behaviour because we want to write our own response body
                            context.HandleResponse();

                            throw new UnauthorizedAccessException("Invalid or missing token.");
                        },
                        // Auth done - permission failed
                        OnForbidden = context =>
                        {
                            throw new UnauthorizedAccessException("You don't have permission to perform this action.");
                        }
                    };
                });

            // register authorization service, need it for .RequireAuthorization in the endpoints (policy checks)
            builder.Services.AddAuthorization();

            // to use it in program.cs
            return builder;
        }
    }
}
