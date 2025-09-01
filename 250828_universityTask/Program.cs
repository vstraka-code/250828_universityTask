using _250828_universityTask.Data;
using _250828_universityTask.Endpoints;
using _250828_universityTask.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// builder.Services.AddAuthentication();
// builder.Services.AddAuthorization();

// configuring Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// registers MediatR with the DI container
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

//AppDbContext is database session that manages tables, uses the connection string
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseAuthorization();
// app.UseAuthentication();

// Register Middleware for Exception Handling
app.UseGlobalExceptionHandling();

// for testing purpose (mocking a fake ClaimsPrincipal
app.Use(async (context, next) =>
{
    var claims = new List<Claim>
    {
        new Claim("ProfessorId", "1"),
        // new Claim("StudentId", "1"),
    };
    var identity = new ClaimsIdentity(claims, "TestAuth");
    context.User = new ClaimsPrincipal(identity);

    await next();
});

// HTTP Endpoints
app.MapStudentEndpoints();

app.Run();
