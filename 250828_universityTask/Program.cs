using _250828_universityTask.Auth;
using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Endpoints;
using _250828_universityTask.Middleware;
using _250828_universityTask.Validators;
using FluentValidation;
using MediatR;

// initializes app builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// AppDbContext is database session that manages tables, uses the connection string
// builder.Services.AddDbContext<AppDbContext>(options =>
// options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<JsonDbContext>();

// registers MediatR with the DI container
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Bind JwtSettings
builder.RegisterAuthentication();
// don't need this is in WebApplicationBuilderExtensions.cs
// builder.Services.AddAuthentication();
// builder.Services.AddAuthorization();

// Register FluentValidation validators
builder.Services.AddValidatorsFromAssemblyContaining<AddStudentRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateStudentRequestValidator>();

// Initializing MemoryCache
builder.Services.AddMemoryCache();
builder.Services.AddScoped<CacheService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // right now only used in cache + exception

// configuring Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "University API",
        Version = "v1"
    });

    // Add JWT bearer auth to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });

    // tells Swagger all endpoints require Bearer token
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// builds it
var app = builder.Build();

// Register Middleware for Exception Handling, can be useful for readability
// app.UseGlobalExceptionHandling();
// app.UseValidationMiddleware();

app.UseMiddleware<ValidationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// extracts + validates the JWT token from Authorization header
app.UseAuthentication();
// checks policies/roles
app.UseAuthorization();

// for testing purpose (mocking a fake ClaimsPrincipal)
// app.Use(async (context, next) =>
// {
// var claims = new List<Claim>
// {
// new Claim("ProfessorId", "1"),
// new Claim("StudentId", "1"),
//    };
// var identity = new ClaimsIdentity(claims, "TestAuth");
// context.User = new ClaimsPrincipal(identity);
// await next();
// });

// HTTP Endpoints
app.MapStudentEndpoints();
app.MapAuthEndpoints();

app.Run();
