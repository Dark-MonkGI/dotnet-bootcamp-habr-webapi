using Habr.BusinessLogic.Interfaces;
using Habr.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Habr.WebApi.DTOs;
using Habr.BusinessLogic.DTOs;

namespace Habr.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly string _secretKey;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _secretKey = configuration["Jwt:SecretKey"];
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
                var user = await _userService.RegisterAsync(registerUserDto.Email, registerUserDto.Password, registerUserDto.IsEmailConfirmed);
                if (registerUserDto.IsEmailConfirmed)
                {
                    var token = GenerateJwtToken(user);
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
                var user = await _userService.AuthenticateAsync(confirmEmailDto.Email, confirmEmailDto.Password);
                if (user == null)
                {
                    return BadRequest("Invalid email or password.");
                }

                if (!confirmEmailDto.IsEmailConfirmed)
                {
                    return BadRequest("Email confirmation failed. Please try again.");
                }

                await _userService.ConfirmEmailAsync(confirmEmailDto.Email, true);
                var token = GenerateJwtToken(user);
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
                var user = await _userService.AuthenticateAsync(authenticateUserDto.Email, authenticateUserDto.Password);
                if (user == null)
                {
                    return BadRequest("Invalid email or password.");
                }

                if (!user.IsEmailConfirmed)
                {
                    return Ok(new { Message = "User authenticated. Please confirm your email." });
                }

                var token = GenerateJwtToken(user);
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

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),

                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
