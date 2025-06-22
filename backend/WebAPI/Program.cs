using Infrastructure.Persistence;
using Infrastructure.Services;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add application services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Detect environment
var env = builder.Environment;

// Conditionally configure EF Core
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

// Add controllers (if you plan to use them now or soon)
builder.Services.AddControllers();

var app = builder.Build();

// Swagger in dev
if (env.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers(); // Enable controller routes

app.Run();
