﻿using Habr.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Habr.BusinessLogic.DTOs;
using Habr.WebApi.Helpers;
using Microsoft.Extensions.Options;

namespace Habr.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtSettings _jwtSettings;

        public UsersController(IUserService userService, IOptions<JwtSettings> jwtSettings)
        {
            _userService = userService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            if (User.Identity.IsAuthenticated)
            {
                return BadRequest("You are already authenticated. Cannot register again.");
            }

            try
            {
                var user = await _userService.RegisterAsync(registerUserDto);

                if (registerUserDto.IsEmailConfirmed)
                {
                    var token = JwtHelper.GenerateJwtToken(user, _jwtSettings.SecretKey, _jwtSettings.TokenLifetimeDays);
                    return Ok(new { Token = token });
                }
                else
                {
                    return Ok(new { Message = "User registered but email not confirmed." });
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto confirmEmailDto)
        {
            if (User.Identity.IsAuthenticated)
            {
                return BadRequest("You are already authenticated. Cannot register again.");
            }

            try
            {
                var user = await _userService.AuthenticateAsync(new AuthenticateUserDto
                {
                    Email = confirmEmailDto.Email,
                    Password = confirmEmailDto.Password
                });

                if (user == null)
                {
                    return BadRequest("Invalid email or password.");
                }

                if (!confirmEmailDto.IsEmailConfirmed)
                {
                    return BadRequest("Email confirmation failed. Please try again.");
                }

                await _userService.ConfirmEmailAsync(confirmEmailDto.Email, true);
                var token = JwtHelper.GenerateJwtToken(user, _jwtSettings.SecretKey, _jwtSettings.TokenLifetimeDays);
                return Ok(new { Token = token, Message = "Email confirmed successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateUserDto authenticateUserDto)
        {
            if (User.Identity.IsAuthenticated)
            {
                return BadRequest("You are already authenticated. Cannot register again.");
            }

            try
            {
                var user = await _userService.AuthenticateAsync(authenticateUserDto);

                if (user == null)
                {
                    return BadRequest("Invalid email or password.");
                }

                if (!user.IsEmailConfirmed)
                {
                    return Ok(new { Message = "User authenticated. Please confirm your email." });
                }

                var token = JwtHelper.GenerateJwtToken(user, _jwtSettings.SecretKey, _jwtSettings.TokenLifetimeDays);
                return Ok(new { Token = token });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
