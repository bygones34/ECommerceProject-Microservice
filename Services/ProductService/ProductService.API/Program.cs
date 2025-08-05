using ProductService.Application.Interfaces;
using ProductService.Application.Services;
using ProductService.Infrastructure.Configurations;
using ProductService.Infrastructure.Repositories;
using MongoDB.Bson.Serialization.Serializers;
using FluentValidation;
using ProductService.Application.DTOs;
using ProductService.Application.Validators;
using ProductService.API.Middlewares;
using Serilog;
using Serilog.Formatting.Json;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        new JsonFormatter(),
        "Logs/log-.json",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(
    new GuidSerializer(MongoDB.Bson.BsonType.String)
);

// Add MongoDB config
builder.Services.Configure<MongoDbSettings>(options =>
{
    var configSection = builder.Configuration.GetSection("MongoDbSettings");

    // Use the one that was provided (if) by the environment
    options.ConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__MongoDb")
                               ?? configSection["ConnectionString"];

    options.DatabaseName = configSection["DatabaseName"];
    options.ProductCollectionName = configSection["ProductCollectionName"];
});

// Dependency Injection
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductClassService>();
builder.Services.AddScoped<IValidator<ProductDto>, ProductDtoValidator>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add Controllers
builder.Services.AddControllers();

builder.Services.AddOpenApi(); // Adds minimal OpenAPI support

// var jwtSettings = builder.Configuration.GetSection("JwtSettings");
// Console.WriteLine("JWT SecretKey: " + jwtSettings["SecretKey"]);

// builder.Services
//     .AddAuthentication("Bearer")
//     .AddJwtBearer("Bearer", options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = jwtSettings["Issuer"],
//             ValidAudience = jwtSettings["Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(
//                 Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
//             NameClaimType = ClaimTypes.NameIdentifier,
//             RoleClaimType = ClaimTypes.Role
//         };
//     });
//
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("AdminOnly", policy =>
//         policy.RequireRole("Admin"));
//
//     options.AddPolicy("UserOrAdmin", policy =>
//         policy.RequireRole("User", "Admin"));
// });

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// app.UseAuthentication();
// app.UseAuthorization();

app.UseMiddleware<ValidationMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
