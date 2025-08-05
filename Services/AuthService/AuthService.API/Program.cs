using AuthService.API.Middlewares;
using AuthService.Application.Services;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthService.Application.Services.Blacklist;

var builder = WebApplication.CreateBuilder(args);

var connStr = builder.Configuration.GetConnectionString("AuthDb")
              ?? Environment.GetEnvironmentVariable("ConnectionStrings__AuthDb");

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AuthDbContext>(opt => 
    opt.UseSqlServer(connStr));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBlacklistService, BlacklistService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();