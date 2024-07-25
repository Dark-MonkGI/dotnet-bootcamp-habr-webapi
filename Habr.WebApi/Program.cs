using Habr.DataAccess;
using Microsoft.EntityFrameworkCore;
using Habr.BusinessLogic.Profiles;
using Habr.WebApi.Extensions;
using Habr.WebApi.Profiles;
using Serilog;
using Serilog.Events;
using Habr.WebApi.Modules;
using Habr.Common;

namespace Habr.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
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

            builder.Services.AddLogging();

            builder.Services.AddGlobalExceptionHandler();

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.Services.AddAuthorization();

            builder.Services.AddSwaggerServices();

            builder.Services.AddBusinessLogicServices();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<DataContext>();
                context.Database.Migrate();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseGlobalExceptionHandler();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.RegisterCommentEndpoints();
            app.RegisterPostEndpoints();
            app.RegisterUserEndpoints();

            app.Run();
        }
    }
}
