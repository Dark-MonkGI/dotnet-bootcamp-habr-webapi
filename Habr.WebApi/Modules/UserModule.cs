using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using Habr.WebApi.Helpers;
using Habr.WebApi.Resources;
using AutoMapper;
using System.Security.Claims;
using Microsoft.Extensions.Options;
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
                    IOptions<JwtSettings> jwtOptions, 
                    IMapper mapper, 
                    ClaimsPrincipal user
                ) =>
            {
                var jwtSettings = jwtOptions.Value;
                if (user.Identity.IsAuthenticated)
                {
                    return Results.BadRequest(Messages.AlreadyAuthenticated);
                }

                var registerUserDto = mapper.Map<RegisterUserDto>(registerUserRequest);
                var registeredUser = await userService.RegisterAsync(registerUserDto);

                if (registerUserRequest.IsEmailConfirmed)
                {
                    var token = JwtHelper.GenerateJwtToken(registeredUser, jwtSettings.SecretKey, jwtSettings.TokenLifetimeDays);
                    return Results.Ok(new { Token = token });
                }
                else
                {
                    return Results.Ok(new { Message = Messages.UserRegisteredEmailNotConfirmed });
                }
            }).WithTags("Users");

            app.MapPost("/api/users/confirm-email", 
                async (
                    [FromBody] ConfirmEmailRequest confirmEmailRequest, 
                    IUserService userService, 
                    IOptions<JwtSettings> jwtOptions, 
                    IMapper mapper, 
                    ClaimsPrincipal user
                ) =>
            {
                var jwtSettings = jwtOptions.Value;
                if (user.Identity.IsAuthenticated)
                {
                    return Results.BadRequest(Messages.AlreadyAuthenticated);
                }

                var authenticateUserDto = mapper.Map<AuthenticateUserDto>(confirmEmailRequest);
                var authenticatedUser = await userService.AuthenticateAsync(authenticateUserDto);

                if (authenticatedUser == null)
                {
                    return Results.BadRequest(Messages.InvalidEmail);
                }

                if (!confirmEmailRequest.IsEmailConfirmed)
                {
                    return Results.BadRequest(Messages.EmailConfirmationFailed);
                }

                await userService.ConfirmEmailAsync(confirmEmailRequest.Email, true);
                var token = JwtHelper.GenerateJwtToken(authenticatedUser, jwtSettings.SecretKey, jwtSettings.TokenLifetimeDays);

                return Results.Ok(new { Token = token, Message = Messages.EmailConfirmedSuccessfully });
            }).WithTags("Users");

            app.MapPost("/api/users/authenticate", 
                async (
                    [FromBody] AuthenticateUserRequest authenticateUserRequest, 
                    IUserService userService, 
                    IOptions<JwtSettings> jwtOptions, 
                    IMapper mapper, 
                    ClaimsPrincipal user
                ) =>
            {
                var jwtSettings = jwtOptions.Value;
                if (user.Identity.IsAuthenticated)
                {
                    return Results.BadRequest(Messages.AlreadyAuthenticated);
                }

                var authenticateUserDto = mapper.Map<AuthenticateUserDto>(authenticateUserRequest);
                var authenticatedUser = await userService.AuthenticateAsync(authenticateUserDto);

                if (authenticatedUser == null)
                {
                    return Results.BadRequest(Messages.InvalidEmail);
                }

                if (!authenticatedUser.IsEmailConfirmed)
                {
                    return Results.Ok(new { Message = Messages.ConfirmYourEmail });
                }

                var token = JwtHelper.GenerateJwtToken(authenticatedUser, jwtSettings.SecretKey, jwtSettings.TokenLifetimeDays);
                return Results.Ok(new { Token = token });
            }).WithTags("Users");

            app.MapGet("/api/users/self", (IUserService userService, ClaimsPrincipal user) =>
            {
                if (!user.Identity.IsAuthenticated)
                {
                    return Results.Unauthorized();
                }

                var email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var userName = email.Split('@')[0];
                return Results.Ok(new { UserName = userName });

            }).WithTags("Users").RequireAuthorization();
        }
    }
}
