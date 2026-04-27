using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NaturalSpaceApi.Application.DTOs.Auth;
using NaturalSpaceApi.Application.DTOs.Channel;
using NaturalSpaceApi.Application.DTOs.Message;
using NaturalSpaceApi.Application.DTOs.Workspace;
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
            builder.Services.AddScoped<IValidator<CreateWorkSpaceRequest>, WorkspaceValidator>();
            builder.Services.AddScoped<IValidator<CreateChannelRequest>, CreateChannelValidator>();
            builder.Services.AddScoped<IValidator<UpdateChannelRequest>, UpdateChannelValidator>();
            builder.Services.AddScoped<IValidator<CreateMessageRequest>, CreateMessageValidator>();
            builder.Services.AddScoped<IValidator<UpdateMessageRequest>, UpdateMessageValidator>();

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();
            builder.Services.AddScoped<IChannelService, ChannelService>();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
            builder.Services.AddScoped<IPermissionService, PermissionService>();

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 10485760; // 10 MB
            });

            var app = builder.Build();

            app.UseExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
