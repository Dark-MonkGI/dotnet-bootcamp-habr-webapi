using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using AutoMapper;
using System.Security.Claims;
using Habr.WebApi.Resources;
using Microsoft.AspNetCore.Mvc;
using Habr.Common;

namespace Habr.WebApi.Modules
{
    public static class PostModule
    {
        public static void RegisterPostEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/posts", async (IPostService postService) =>
            {
                var posts = await postService.GetAllPublishedPosts();

                return posts.Any() ? Results.Ok(posts) : Results.NotFound(Messages.NoPostsFound);
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi()
            .WithTags(Constants.Tags.PostsTag)
            .RequireAuthorization(Constants.Policies.UserPolicy);

            app.MapGet("/api/posts/drafts", async (IPostService postService, ClaimsPrincipal user) =>
            {
                var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
                var posts = await postService.GetUserDraftPosts(userId);

                return posts.Any() ? Results.Ok(posts) : Results.NotFound(Messages.NoDraftPosts);
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi()
            .WithTags(Constants.Tags.PostsTag)
            .RequireAuthorization(Constants.Policies.UserPolicy);

            app.MapPost("/api/posts", 
                async (
                    [FromBody] CreatePostRequest createPostRequest,
                    IPostService postService,
                    ClaimsPrincipal user,
                    IMapper mapper
                ) =>
            {
                var createPostDto = mapper.Map<CreatePostDto>(createPostRequest);
                createPostDto.UserId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
                var post = await postService.CreatePost(createPostDto);

                return Results.Created($"/api/posts/{post.Id}", post);
            })
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi()
            .WithTags(Constants.Tags.PostsTag)
            .RequireAuthorization(Constants.Policies.UserPolicy);

            app.MapPut("/api/posts/{postId}", 
                async (
                    [FromRoute] int postId,
                    UpdatePostRequest updatePostRequest,
                    IPostService postService,
                    ClaimsPrincipal user,
                    IMapper mapper
                ) =>
            {
                var updatePostDto = mapper.Map<UpdatePostDto>(updatePostRequest);
                updatePostDto.UserId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
                updatePostDto.PostId = postId;
                await postService.UpdatePost(updatePostDto);

                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi()
            .WithTags(Constants.Tags.PostsTag)
            .RequireAuthorization(Constants.Policies.UserPolicy);

            app.MapDelete("/api/posts/{postId}", async ([FromRoute] int postId, IPostService postService, ClaimsPrincipal user) =>
            {
                var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
                await postService.DeletePost(postId, userId);

                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi()
            .WithTags(Constants.Tags.PostsTag)
            .RequireAuthorization(Constants.Policies.UserPolicy);

            app.MapPost("/api/posts/{postId}/publish", async ([FromRoute] int postId, IPostService postService, ClaimsPrincipal user) =>
            {
                var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
                await postService.PublishPostAsync(postId, userId);

                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi()
            .WithTags(Constants.Tags.PostsTag)
            .RequireAuthorization(Constants.Policies.UserPolicy);

            app.MapPost("/api/posts/{postId}/move-to-draft", async ([FromRoute] int postId, IPostService postService, ClaimsPrincipal user) =>
            {
                var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
                await postService.MovePostToDraftAsync(postId, userId);

                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi()
            .WithTags(Constants.Tags.PostsTag)
            .RequireAuthorization(Constants.Policies.UserPolicy);

            app.MapGet("/api/posts/{postId}", async ([FromRoute] int postId, IPostService postService) =>
            {
                var postDetails = await postService.GetPostDetailsAsync(postId);

                return Results.Ok(postDetails);
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi()
            .WithTags(Constants.Tags.PostsTag)
            .RequireAuthorization(Constants.Policies.UserPolicy);

            app.MapPut("/api/admin/posts/{postId}",
                async (
                    [FromRoute] int postId,
                    UpdatePostRequest updatePostRequest,
                    IPostService postService,
                    IMapper mapper
                ) =>
                {
                    var updatePostDto = mapper.Map<UpdatePostDto>(updatePostRequest);
                    updatePostDto.PostId = postId;
                    await postService.UpdatePostAsAdmin(updatePostDto);

                    return Results.Ok();
                })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi()
            .WithTags(Constants.Tags.AdminTag)
            .RequireAuthorization(Constants.Policies.AdminPolicy);

            app.MapDelete("/api/admin/posts/{postId}", async ([FromRoute] int postId, IPostService postService) =>
            {
                await postService.DeletePostAsAdmin(postId);

                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi()
            .WithTags(Constants.Tags.AdminTag)
            .RequireAuthorization(Constants.Policies.AdminPolicy);
        }
    }
}
