using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NaturalSpaceApi.Application.DTOs.Auth;
using NaturalSpaceApi.Application.Interfaces;
using NaturalSpaceApi.Application.Services;
using NaturalSpaceApi.Application.Validators;
using NaturalSpaceApi.Extensions;
using NaturalSpaceApi.Infrastructure.Data.Context;
using NaturalSpaceApi.Infrastructure.Options;
using NaturalSpaceApi.Middleware;

namespace NaturalSpaceApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddDbContext<NaturalSpaceContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });

            builder.Services.Configure<JwtOptions>(
                    builder.Configuration.GetSection("Jwt"));

            builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
            builder.Services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();

            var app = builder.Build();

            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
