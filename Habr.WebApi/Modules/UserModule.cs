using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Habr.WebApi.Modules
{
    public static class UserModule
    {
        public static void RegisterUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/users/register",
                async (
                    [FromBody] RegisterUserRequest registerUserRequest,
                    IUserService userService,
                    IMapper mapper,
                    ClaimsPrincipal user
                ) =>
            {
                var registerUserDto = mapper.Map<RegisterUserDto>(registerUserRequest);
                var token = await userService.RegisterUserAsync(registerUserDto, user);

                return Results.Ok(new { Token = token });
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi()
            .WithTags("Users");

            app.MapPost("/api/users/confirm-email",
                async (
                    [FromBody] ConfirmEmailRequest confirmEmailRequest,
                    IUserService userService,
                    IMapper mapper,
                    ClaimsPrincipal user
                ) =>
            {
                var authenticateUserDto = mapper.Map<AuthenticateUserDto>(confirmEmailRequest);
                var (token, message) = await userService.ConfirmEmailAsync(authenticateUserDto, user);
                return Results.Ok(new { Token = token, Message = message });
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi()
            .WithTags("Users");

            app.MapPost("/api/users/authenticate",
                async (
                    [FromBody] AuthenticateUserRequest authenticateUserRequest,
                    IUserService userService,
                    IMapper mapper,
                    ClaimsPrincipal user
                ) =>
            {
                var authenticateUserDto = mapper.Map<AuthenticateUserDto>(authenticateUserRequest);
                var (token, message) = await userService.AuthenticateUserAsync(authenticateUserDto, user);
                return Results.Ok(new { Token = token, Message = message });
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi()
            .WithTags("Users");

            app.MapGet("/api/users/self", (IUserService userService, ClaimsPrincipal user) =>
            {
                if (!user.Identity.IsAuthenticated)
                {
                    return Results.Unauthorized();
                }

                var email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var userName = email.Split('@')[0];
                return Results.Ok(new { UserName = userName });

            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi()
            .WithTags("Users")
            .RequireAuthorization();
        }
    }
}
