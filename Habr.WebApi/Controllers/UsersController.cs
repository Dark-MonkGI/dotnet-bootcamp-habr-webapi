using Habr.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Habr.BusinessLogic.DTOs;
using Habr.WebApi.Helpers;
using Microsoft.Extensions.Options;
using Habr.WebApi.Resources;

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
                return BadRequest(Messages.AlreadyAuthenticated);
            }

            var user = await _userService.RegisterAsync(registerUserDto);

            if (registerUserDto.IsEmailConfirmed)
            {
                var token = JwtHelper.GenerateJwtToken(user, _jwtSettings.SecretKey, _jwtSettings.TokenLifetimeDays);
                return Ok(new { Token = token });
            }
            else
            {
                return Ok(new { Message = Messages.UserRegisteredEmailNotConfirmed });
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto confirmEmailDto)
        {
            if (User.Identity.IsAuthenticated)
            {
                return BadRequest(Messages.AlreadyAuthenticated);
            }

            var user = await _userService.AuthenticateAsync(new AuthenticateUserDto
            {
                Email = confirmEmailDto.Email,
                Password = confirmEmailDto.Password
            });

            if (user == null)
            {
                return BadRequest(Messages.InvalidEmail);
            }

            if (!confirmEmailDto.IsEmailConfirmed)
            {
                return BadRequest(Messages.EmailConfirmationFailed);
            }

            await _userService.ConfirmEmailAsync(confirmEmailDto.Email, true);
            var token = JwtHelper.GenerateJwtToken(user, _jwtSettings.SecretKey, _jwtSettings.TokenLifetimeDays);

            return Ok(new { Token = token, Message = Messages.EmailConfirmedSuccessfully });
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateUserDto authenticateUserDto)
        {
            if (User.Identity.IsAuthenticated)
            {
                return BadRequest(Messages.AlreadyAuthenticated);
            }

            var user = await _userService.AuthenticateAsync(authenticateUserDto);

            if (user == null)
            {
                return BadRequest(Messages.InvalidEmail);
            }

            if (!user.IsEmailConfirmed)
            {
                return Ok(new { Message = Messages.ConfirmYourEmail });
            }

            var token = JwtHelper.GenerateJwtToken(user, _jwtSettings.SecretKey, _jwtSettings.TokenLifetimeDays);
            return Ok(new { Token = token });
        }
    }
}
