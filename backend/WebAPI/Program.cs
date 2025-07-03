using Application.Interfaces;
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

// 1. JWT Bearer Authentication
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero // ⏱️ Gör JWT-validering striktare
        };
    });

// 2. Swagger med JWT-stöd
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
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

// 3. Registrera tjänster
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IMissionService, MissionService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();

// 4. Databaskontext
var env = builder.Environment;
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var conn = builder.Configuration.GetConnectionString("DefaultConnection")!;
    if (env.IsDevelopment())
        options.UseSqlite(conn, b => b.MigrationsAssembly("Infrastructure"));
    else
        options.UseSqlServer(conn, b => b.MigrationsAssembly("Infrastructure"));
});

// 5. Authorization & Controllers
builder.Services.AddAuthorization();
builder.Services.AddControllers();

// 6. CORS (separerad för dev/prod)
builder.Services.AddCors(options =>
{
    if (env.IsDevelopment())
    {
        options.AddDefaultPolicy(policy =>
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod());
    }
    else
    {
        options.AddDefaultPolicy(policy =>
            policy.WithOrigins("https://volontarplattform.se") // Uppdatera till riktig domän
                  .AllowAnyHeader()
                  .AllowAnyMethod());
    }
});

// 7. Bygg pipeline
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

// 8. Dev seed
if (env.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var ctx      = services.GetRequiredService<AppDbContext>();
    var logger   = services.GetRequiredService<ILogger<Program>>();
    ctx.Database.Migrate();
    await AppDbContextSeeder.SeedAsync(ctx, logger);
}

app.Run();
