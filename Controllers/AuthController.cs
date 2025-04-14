using Microsoft.AspNetCore.Mvc;
using TSU360.DTOs.Auth;
using TSU360.Services.Interfaces;
using TSU360.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TSU360.Models.Entities;
using TSU360.Services.Implementations;
using TSU360.Models.DTO_s;

namespace TSU360.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<RegisterDto> _validator;
        private readonly ITokenBlacklistService _tokenBlackListService;

        public AuthController(
            IUserService UserService,
            IValidator<RegisterDto> validator,
            ITokenBlacklistService tokenBlackListService)
        {
            _userService = UserService;
            _validator = validator;
            _tokenBlackListService = tokenBlackListService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var validationResult = await _validator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var response = await _userService.RegisterAsync(registerDto);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _userService.LoginAsync(loginDto);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var profile = await _userService.GetUserProfileAsync(userId);
            return Ok(profile);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required");

            var expiration = DateTime.UtcNow.AddMinutes(60); // Or extract from JWT
            await _tokenBlackListService.BlacklistTokenAsync(token, expiration);

            return Ok(new { message = "Logged out successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("promote")]
        public async Task<IActionResult> PromoteUserToCurator([FromBody] PromoteUserDto dto)
        {
            try
            {
                await _userService.PromoteToCuratorAsync(dto.Email);
                return Ok(new { Message = $"User {dto.Email} promoted to Curator successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }


    }

}