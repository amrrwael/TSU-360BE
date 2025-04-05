// ITokenService.cs
using TSU360.Models.Entities;

namespace TSU360.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}