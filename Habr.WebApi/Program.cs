using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Services;
using Habr.DataAccess;
using Microsoft.EntityFrameworkCore;
using Habr.BusinessLogic.Profiles;
using Habr.WebApi.Extensions;
using Habr.WebApi.Profiles;
using Serilog;
using Serilog.Events;
using Habr.WebApi.Modules;

namespace Habr.WebApi
{
    public class Program
    {
        private const string LogFilePath = "logs/log.txt";

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Async(a => a.File(LogFilePath, rollingInterval: RollingInterval.Day))
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

            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddLogging();

            builder.Services.AddGlobalExceptionHandler();

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.Services.AddAuthorization();

            builder.Services.AddSwaggerServices();

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
