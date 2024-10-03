using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Ecommerce.Services.AuthService.Intefaces;
using FluentValidation.Results;
using Ecommerce.Services.AuthService.DTO;
using static Ecommerce.Services.AuthService.DTO.LoginCredentialsDto;
using static Ecommerce.Services.AuthService.DTO.RefreshTokenDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [SwaggerTag("User authentication and registration")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Login a user")]
        [SwaggerResponse(200, "The user was logged in successfully", typeof(LoginResultDto))]
        [SwaggerResponse(400, "Validation failed", typeof(ValidationProblemDetails))]
        [SwaggerResponse(404, "Not Found - user does not exist", typeof(ProblemDetails))]
        public async Task<ActionResult<LoginResultDto>> Login([FromBody] LoginCredentialsDto loginDto)
        {
            var validationResult = new LoginCredentialsDtoValidator().Validate(loginDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            var result = await _authService.LoginAsync(loginDto);

            return Ok(result);
        }

        [HttpPost("refresh")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Refresh a user's token")]
        [SwaggerResponse(200, "The user's token was refreshed successfully", typeof(LoginResultDto))]
        [SwaggerResponse(400, "Token is invalid", typeof(ProblemDetails))]
        public async Task<ActionResult<LoginResultDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var validationResult = new RefreshTokenDtoValidator().Validate(refreshTokenDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            return await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            var token = await HttpContext.GetTokenAsync("access_token") ?? throw new Exception("Token is invalid");
            await _authService.LogoutAsync(token);
            return Ok();
        }
    }
}