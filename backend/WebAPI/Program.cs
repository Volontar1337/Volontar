using Infrastructure.Persistence;
using Infrastructure.Services;
using Application.Interfaces;
using Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using WebAPI.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// AUTHENTICATION & COOKIE CONFIGURATION
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/api/auth/login"; // API login endpoint
        options.LogoutPath = "/api/auth/logout";
        options.Cookie.Name = "volunteer_platform_auth";
        options.Cookie.HttpOnly = true; // Prevents JS access to the cookie
        options.Cookie.SameSite = SameSiteMode.Lax; // Use Strict for extra security if possible
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Always use Secure in production!
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Cookie lifetime
        options.SlidingExpiration = true; // Refreshes expiration on activity
        options.Events.OnRedirectToLogin = context =>
        {
            // For APIs: Return 401 instead of redirecting to login page
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });

// SWAGGER (OpenAPI docs)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// YOUR APPLICATION SERVICES
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IMissionService, MissionService>();

// ENVIRONMENT AND DATABASE SETUP
var env = builder.Environment;

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (env.IsDevelopment())
    {
        options.UseSqlite(connectionString, b => b.MigrationsAssembly("Infrastructure"));
    }
    else
    {
        options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Infrastructure"));
    }
});

// AUTHORIZATION (for [Authorize] attributes)
builder.Services.AddAuthorization();

// CONTROLLER SUPPORT
builder.Services.AddControllers();

// CORS CONFIGURATION - required for cookies with frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5102") // Your frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Must be set to allow cookies
    });
});

var app = builder.Build();

// SWAGGER only in development
if (env.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors();           // Should come before auth!
app.UseAuthentication(); // Enables cookie/session authentication
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

// SEED DATA (development only)
if (env.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        await AppDbContextSeeder.SeedAsync(context, logger);
    }
}

app.Run();
