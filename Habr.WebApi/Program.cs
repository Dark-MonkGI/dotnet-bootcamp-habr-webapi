using Habr.DataAccess;
using Microsoft.EntityFrameworkCore;
using Habr.BusinessLogic.Profiles;
using Habr.WebApi.Extensions;
using Habr.WebApi.Profiles;
using Serilog;
using Serilog.Events;
using Habr.WebApi.Modules;
using Habr.Common;
using Microsoft.AspNetCore.Identity;
using Habr.DataAccess.Entities;

namespace Habr.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Async(a => a.File(Constants.Log.LogFilePath, rollingInterval: RollingInterval.Day))
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            builder.Services.AddAutoMapper(
                typeof(PostProfile), 
                typeof(CommentProfile), 
                typeof(UserProfile),
                typeof(ExceptionProfile),
                typeof(WebApiMappingProfile)
                );

            builder.Services.Configure<BusinessLogic.Helpers.JwtSettings>(builder.Configuration.GetSection("Jwt"));

            builder.Services.AddDbContext<DataContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<User, IdentityRole<int>>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddLogging();

            builder.Services.AddGlobalExceptionHandler();

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.Services.AddAuthorization();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.Policies.UserPolicy, policy => policy.RequireRole(Constants.Roles.User, 
                    Constants.Roles.Admin));
                options.AddPolicy(Constants.Policies.AdminPolicy, policy => policy.RequireRole(Constants.Roles.Admin));
            });

            builder.Services.AddSwaggerServices();

            builder.Services.AddBusinessLogicServices();

            builder.Services.AddApiVersioningServices();

            var app = builder.Build();

            await app.Services.InitializeDatabaseAndRolesAsync();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Habr API v1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Habr API v2");
                });
            }

            app.UseGlobalExceptionHandler();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            var apiVersionSet = app.CreateApiVersionSet();

            app.RegisterCommentEndpoints(apiVersionSet);
            app.RegisterPostEndpoints(apiVersionSet);
            app.RegisterUserEndpoints(apiVersionSet);

            await app.RunAsync();
        }
    }
}
