using TSU360.Database;
using TSU360.Models.Entities;
using Microsoft.EntityFrameworkCore;


public class TokenBlacklistService : ITokenBlacklistService
{
    private readonly ApplicationDbContext _context;

    public TokenBlacklistService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task BlacklistTokenAsync(string token, DateTime expiration)
    {
        _context.TokenBlacklists.Add(new TokenBlacklist
        {
            Token = token,
            Expiration = expiration
        });
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsTokenBlacklistedAsync(string token)
    {
        return await _context.TokenBlacklists.AnyAsync(t => t.Token == token);
    }
}
