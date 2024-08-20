using Habr.WebApi.Resources;

namespace Habr.WebApi.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = Resources.Messages.SwaggerSecurityDefinitionDescription,
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement{
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme{
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference{
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
                c.SwaggerDoc(SwaggerDescriptions.HabrApiV1Version, new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = SwaggerDescriptions.HabrApiV1Version,
                    Title = SwaggerDescriptions.HabrApiV1Title,
                    Description = SwaggerDescriptions.HangfireDashboardDescriptionV1
                });
                c.SwaggerDoc(SwaggerDescriptions.HabrApiV2Version, new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = SwaggerDescriptions.HabrApiV2Version,
                    Title = SwaggerDescriptions.HabrApiV2Title,
                    Description = SwaggerDescriptions.HangfireDashboardDescriptionV2
                });
            });

            return services;
        }
    }
}
