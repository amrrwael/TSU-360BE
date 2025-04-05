using Microsoft.AspNetCore.Mvc;
using TSU360.DTOs.Auth;
using TSU360.Services.Interfaces;
using TSU360.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TSU360.Models.Entities;
using TSU360.Services.Implementations;

namespace TSU360.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _authService;
        private readonly IValidator<RegisterDto> _validator;

        public AuthController(
            IUserService authService,
            IValidator<RegisterDto> validator)
        {
            _authService = authService;
            _validator = validator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var validationResult = await _validator.ValidateAsync(registerDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var response = await _authService.RegisterAsync(registerDto);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var profile = await _authService.GetUserProfileAsync(userId);
            return Ok(profile);
        }
    }
    
}