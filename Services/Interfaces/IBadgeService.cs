using TSU360.Models.DTOs;
using TSU360.Models.Entities;

namespace TSU360.Services.Interfaces
{
    public interface IBadgeService
    {
        Task<IEnumerable<BadgeDTO>> GetAllBadgesAsync();
        Task<BadgeDTO> GetBadgeByIdAsync(Guid id);
        Task<BadgeDTO> CreateBadgeAsync(CreateBadgeDTO badgeDto);
        Task<BadgeDTO> UpdateBadgeAsync(Guid id, UpdateBadgeDTO badgeDto);
        Task<bool> DeleteBadgeAsync(Guid id);
    }
}