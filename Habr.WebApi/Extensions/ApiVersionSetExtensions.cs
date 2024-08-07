using Asp.Versioning.Builder;
using Asp.Versioning;

namespace Habr.WebApi.Extensions
{
    public static class ApiVersionSetExtensions
    {
        public static ApiVersionSet CreateApiVersionSet(this WebApplication app)
        {
            return app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .HasApiVersion(new ApiVersion(2, 0))
                .ReportApiVersions()
                .Build();
        }
    }
}