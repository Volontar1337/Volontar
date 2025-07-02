using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ────────────────────────────────────────────────────────────────────
// 1. JWT BEARER AUTHENTICATION
// ────────────────────────────────────────────────────────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// ────────────────────────────────────────────────────────────────────
// 2. SWAGGER (API-dokumentation) med JWT-stöd
// ────────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // JWT Bearer-definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.ApiKey,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Skriv: Bearer {din JWT-token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme, 
                    Id   = "Bearer" 
                }
            },
            Array.Empty<string>()
        }
    });
});

// ────────────────────────────────────────────────────────────────────
// 3. REGISTER SERVICES
// ────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IMissionService, MissionService>();

// ────────────────────────────────────────────────────────────────────
// 4. DATABASE CONTEXT
// ────────────────────────────────────────────────────────────────────
var env = builder.Environment;
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("DefaultConnection")!;

    if (env.IsDevelopment())
        options.UseSqlite(conn, b => b.MigrationsAssembly("Infrastructure"));
    else
        options.UseSqlServer(conn, b => b.MigrationsAssembly("Infrastructure"));
});

// ────────────────────────────────────────────────────────────────────
// 5. AUTHORIZATION & CONTROLLERS
// ────────────────────────────────────────────────────────────────────
builder.Services.AddAuthorization();
builder.Services.AddControllers();

// ────────────────────────────────────────────────────────────────────
// 6. CORS (för eventuellt annat än cookies – nu valfritt för JWT-API)
// ────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ────────────────────────────────────────────────────────────────────
// 7. BUILD PIPELINE
// ────────────────────────────────────────────────────────────────────
var app = builder.Build();

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

// ────────────────────────────────────────────────────────────────────
// 8. OPTIONAL SEED DATA IN DEVELOPMENT
// ────────────────────────────────────────────────────────────────────
if (env.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context  = services.GetRequiredService<AppDbContext>();
    var logger   = services.GetRequiredService<ILogger<Program>>();

    context.Database.Migrate();
    await AppDbContextSeeder.SeedAsync(context, logger);
}

app.Run();
