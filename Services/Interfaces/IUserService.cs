using TSU360.DTOs.Auth;
using System.Threading.Tasks;
using TSU360.DTOs;

namespace TSU360.Services.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<UserProfileDto> GetUserProfileAsync(string userId);
        Task PromoteToCuratorAsync(string email);

    }
}