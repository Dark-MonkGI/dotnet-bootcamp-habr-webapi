using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Habr.Common;
using Asp.Versioning.Builder;
using Habr.WebApi.Filters;

namespace Habr.WebApi.Modules
{
    public static class CommentModule
    {
        public static void RegisterCommentEndpoints(this IEndpointRouteBuilder app, ApiVersionSet apiVersionSet)
        {
            app.MapPost("/api/v{version:apiVersion}/comments/{postId}", 
                async (
                    [FromBody] AddCommentRequest addCommentRequest,
                    int postId,
                    ICommentService commentService, 
                    ClaimsPrincipal user
                ) =>
            {
                var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
                var comment = await commentService.AddComment(new AddCommentDto
                {
                    UserId = userId,
                    PostId = postId,
                    Text = addCommentRequest.Text
                });

                return Results.Created($"/api/comments/{comment.Id}", comment);
            })
            .AddEndpointFilter<ValidationFilter<AddCommentRequest>>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(1.0)
            .WithOpenApi()
            .WithTags(Constants.Tags.CommentsTag)
            .RequireAuthorization(Constants.Policies.UserPolicy);

            app.MapPost("/api/v{version:apiVersion}/comments/{parentCommentId}/reply", 
                async (
                    [FromBody] AddReplyRequest addReplyRequest,
                    int parentCommentId,
                    ICommentService commentService, 
                    ClaimsPrincipal user
                ) =>
            {
                var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
                var comment = await commentService.AddReply(new AddReplyDto
                {
                    UserId = userId,
                    ParentCommentId = parentCommentId,
                    Text = addReplyRequest.Text
                });

                return Results.Created($"/api/comments/{comment.Id}", comment);
            })
            .AddEndpointFilter<ValidationFilter<AddReplyRequest>>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(1.0)
            .WithOpenApi()
            .WithTags(Constants.Tags.CommentsTag)
            .RequireAuthorization(Constants.Policies.UserPolicy);

            app.MapDelete("/api/v{version:apiVersion}/comments/{commentId}", 
                async (int commentId, ICommentService commentService, ClaimsPrincipal user) =>
            {
                var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
                await commentService.DeleteComment(commentId, userId);
                
                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(1.0)
            .WithOpenApi()
            .WithTags(Constants.Tags.CommentsTag)
            .RequireAuthorization(Constants.Policies.UserPolicy);

            app.MapDelete("/api/v{version:apiVersion}/admin/comments/{commentId}", 
                async (int commentId, ICommentService commentService) =>
            {
                await commentService.DeleteCommentAsAdmin(commentId);

                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithApiVersionSet(apiVersionSet)
            .MapToApiVersion(1.0)
            .WithOpenApi()
            .WithTags(Constants.Tags.AdminTag)
            .RequireAuthorization(Constants.Policies.AdminPolicy);
        }
    }
}
