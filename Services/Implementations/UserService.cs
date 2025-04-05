using Microsoft.AspNetCore.Identity;
using TSU360.DTOs.Auth;
using TSU360.Models.Entities;
using TSU360.Services.Interfaces;
using System.Threading.Tasks;
using System;
using TSU360.DTOs;
using TSU360.Models.Enums;

namespace TSU360.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public UserService(
            UserManager<User> userManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }


        // UserService.cs
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Birthday = registerDto.Birthday,
                Faculty = registerDto.Faculty,
                Year = registerDto.Year,
                UserRole = UserRole.Attendee // Default role
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            // Add to Identity role system
            await _userManager.AddToRoleAsync(user, UserRole.Attendee.ToString());

            return new AuthResponseDto
            {
                Token = _tokenService.GenerateToken(user),
                Expiration = DateTime.UtcNow.AddMinutes(60),
                UserId = user.Id,
                Email = user.Email,
                UserType = UserRole.Attendee.ToString() // Return as string
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                throw new Exception("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);

            return new AuthResponseDto
            {
                Token = _tokenService.GenerateToken(user), // Removed await
                Expiration = DateTime.UtcNow.AddMinutes(60),
                UserId = user.Id,
                Email = user.Email,
                UserType = roles.FirstOrDefault()
            };
        }
        public async Task<UserProfileDto> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            return new UserProfileDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Birthday = user.Birthday,
                Faculty = user.Faculty,
                Year = user.Year,
            };
        }
    }
}