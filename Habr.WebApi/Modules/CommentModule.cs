using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Habr.WebApi.Modules
{
    public static class CommentModule
    {
        public static void RegisterCommentEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/comments/{postId}", 
                async (
                    int postId, 
                    [FromBody] AddCommentRequest addCommentRequest, 
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
            }).WithTags("Comments").RequireAuthorization();

            app.MapPost("/api/comments/{parentCommentId}/reply", 
                async (
                    int parentCommentId,
                    [FromBody] AddReplyRequest addReplyRequest, 
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
            }).WithTags("Comments").RequireAuthorization();

            app.MapDelete("/api/comments/{commentId}", async (int commentId, ICommentService commentService, ClaimsPrincipal user) =>
            {
                var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
                await commentService.DeleteComment(commentId, userId);
                
                return Results.Ok();
            }).WithTags("Comments").RequireAuthorization();
        }
    }
}
