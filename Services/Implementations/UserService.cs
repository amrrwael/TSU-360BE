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

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {

            if (!Enum.TryParse<Faculty>(registerDto.Faculty, true, out var faculty))
            {
                throw new ArgumentException($"Invalid faculty value: {registerDto.Faculty}");
            }

            if (!Enum.TryParse<Degree>(registerDto.Degree, true, out var degree))
            {
                throw new ArgumentException($"Invalid degree value: {registerDto.Degree}");
            }
            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Birthday = registerDto.Birthday,
                Faculty = faculty,
                Degree = degree,
                Year = registerDto.Year,
                UserRole = UserRole.Attendee // Default role
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            // Assign both UserRole and Identity Role
            await _userManager.AddToRoleAsync(user, user.UserRole.ToString());

            return new AuthResponseDto
            {
                Token = _tokenService.GenerateToken(user),
                Expiration = DateTime.UtcNow.AddMinutes(60),
                UserId = user.Id,
                Email = user.Email,
                UserType = user.UserRole.ToString()
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                throw new Exception("Invalid credentials");

            // Get the highest-priority role (or first role)
            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault(); // or use logic to determine primary role

            return new AuthResponseDto
            {
                Token = _tokenService.GenerateToken(user),
                Expiration = DateTime.UtcNow.AddMinutes(60),
                UserId = user.Id,
                Email = user.Email,
                UserType = user.UserRole.ToString() // Use the UserRole property directly
            };
        }
        public async Task<UserProfileDto> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault();

            return new UserProfileDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Birthday = user.Birthday,
                Faculty = user.Faculty.ToString(),
                Degree = user.Degree.ToString(),
                Year = user.Year,
            };
        }
        public async Task PromoteToCuratorAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("User not found");

            // Remove all existing roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Add new role to both systems
            await _userManager.AddToRoleAsync(user, UserRole.Curator.ToString());
            user.UserRole = UserRole.Curator;
            await _userManager.UpdateAsync(user);
        }


    }
}