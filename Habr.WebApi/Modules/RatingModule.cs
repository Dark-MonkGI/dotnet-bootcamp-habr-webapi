using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Asp.Versioning.Builder;
using Habr.Common;
using AutoMapper;

namespace Habr.WebApi.Modules
{
    public static class RatingModule
    {
        public static void RegisterRatingEndpoints(this IEndpointRouteBuilder app, ApiVersionSet apiVersionSet)
        {
            app.MapPost("/api/v{version:apiVersion}/posts/rate",
                async (
                    [FromBody] RatePostRequest ratePostRequest,
                    IRatingService ratingService,
                    IMapper mapper,
                    ClaimsPrincipal user) =>
                {
                    var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

                    var ratePostDto = mapper.Map<RatePostDto>(ratePostRequest);
                    ratePostDto.UserId = userId;

                    await ratingService.RatePostAsync(ratePostDto);

                    return Results.Ok();
                })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(1.0)
            .WithOpenApi()
            .WithTags(Constants.Tags.RatingsTag)
            .RequireAuthorization(Constants.Policies.UserPolicy);
        }
    }
}
