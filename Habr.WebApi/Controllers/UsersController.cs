using Habr.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Habr.BusinessLogic.DTOs;
using Habr.WebApi.Helpers;
using Microsoft.Extensions.Options;
using Habr.WebApi.Resources;
using AutoMapper;

namespace Habr.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtSettings _jwtSettings;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IOptions<JwtSettings> jwtSettings, IMapper mapper)
        {
            _userService = userService;
            _jwtSettings = jwtSettings.Value;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest registerUserRequest)
        {
            if (User.Identity.IsAuthenticated)
            {
                return BadRequest(Messages.AlreadyAuthenticated);
            }

            var registerUserDto = _mapper.Map<RegisterUserDto>(registerUserRequest);
            var user = await _userService.RegisterAsync(registerUserDto);

            if (registerUserRequest.IsEmailConfirmed)
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
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest confirmEmailRequest)
        {
            if (User.Identity.IsAuthenticated)
            {
                return BadRequest(Messages.AlreadyAuthenticated);
            }

            var authenticateUserDto = _mapper.Map<AuthenticateUserDto>(confirmEmailRequest);
            var user = await _userService.AuthenticateAsync(authenticateUserDto);

            if (user == null)
            {
                return BadRequest(Messages.InvalidEmail);
            }

            if (!confirmEmailRequest.IsEmailConfirmed)
            {
                return BadRequest(Messages.EmailConfirmationFailed);
            }

            await _userService.ConfirmEmailAsync(confirmEmailRequest.Email, true);
            var token = JwtHelper.GenerateJwtToken(user, _jwtSettings.SecretKey, _jwtSettings.TokenLifetimeDays);

            return Ok(new { Token = token, Message = Messages.EmailConfirmedSuccessfully });
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateUserRequest authenticateUserRequest)
        {
            if (User.Identity.IsAuthenticated)
            {
                return BadRequest(Messages.AlreadyAuthenticated);
            }

            var authenticateUserDto = _mapper.Map<AuthenticateUserDto>(authenticateUserRequest);
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
