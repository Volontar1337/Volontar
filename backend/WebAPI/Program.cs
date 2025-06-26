using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// =========================
// 1. COOKIE AUTHENTICATION
// =========================
// Välj ett gemensamt namn för authentication scheme: "CookieAuth"
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        // Sätt enhetliga och tydliga cookie-alternativ för API
        options.LoginPath = "/api/auth/login";     // API endpoint, EJ någon MVC-view
        options.LogoutPath = "/api/auth/logout";   // API endpoint för logout
        options.Cookie.Name = "volunteer_platform_auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax; // Eller Strict om frontend klarar det!
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Tvinga HTTPS!
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context =>
        {
            // API:er ska INTE redirecta vid 401 utan svara med 401-status
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });

// =========================
// 2. SWAGGER (API-dokumentation)
// =========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =========================
// 3. ERA APPLICATION SERVICES
// =========================
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IMissionService, MissionService>();

// =========================
// 4. DBContext: Environment-aware setup
// =========================
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

// =========================
// 5. AUTHORIZATION
// =========================
builder.Services.AddAuthorization();

// =========================
// 6. CONTROLLERS
// =========================
builder.Services.AddControllers();

// =========================
// 7. CORS FÖR COOKIES
// =========================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5102", "http://localhost:8081") // Använd rätt port för er frontend!
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // NÖDVÄNDIGT för cookies!
    });
});

// =========================
// 8. APP PIPELINE & MIDDLEWARES
// =========================
var app = builder.Build();

if (env.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors();           // Viktigt: CORS före Auth
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();

// =========================
// 9. SEED DATA I DEV
// =========================
if (env.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        context.Database.Migrate();
        await AppDbContextSeeder.SeedAsync(context, logger);
    }
}

app.Run();