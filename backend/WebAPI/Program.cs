using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

// Swagger in dev
if (env.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
