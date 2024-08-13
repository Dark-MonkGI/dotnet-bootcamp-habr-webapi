using Asp.Versioning.Builder;
using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using Habr.Common;
using Habr.WebApi.Resources;

namespace Habr.WebApi.Modules
{
    public static class PostModuleV2
    {
        public static void RegisterPostEndpointsV2(this IEndpointRouteBuilder app, ApiVersionSet apiVersionSet)
        {
            app.MapGet("/api/v{version:apiVersion}/posts",
                async (
                    IPostService postService,
                    [AsParameters] PaginationRequest paginationRequest) =>
                {
                    var paginatedPosts = await postService.GetAllPublishedPostsV2(paginationRequest.PageNumber, paginationRequest.PageSize);

                    return paginatedPosts.Any() ? Results.Ok(paginatedPosts) : Results.NotFound(Messages.NoPostsFound);
                })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(2.0)
            .WithOpenApi()
            .WithTags(Constants.Tags.PostsTag)
            .RequireAuthorization(Constants.Policies.UserPolicy);
        }
    }
}
