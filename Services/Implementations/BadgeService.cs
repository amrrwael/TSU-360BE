// Services/Implementations/BadgeService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TSU360.Database;
using TSU360.Models.DTOs;
using TSU360.Models.Entities;
using TSU360.Services.Interfaces;

namespace TSU360.Services.Implementations
{
    public class BadgeService : IBadgeService
    {
        private readonly ApplicationDbContext _context;

        public BadgeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BadgeDTO>> GetAllBadgesAsync()
        {
            return await _context.Badges
                .Select(b => new BadgeDTO
                {
                    Id = b.Id,
                    Name = b.Name,
                    ImageUrl = b.ImageUrl,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<BadgeDTO> GetBadgeByIdAsync(Guid id)
        {
            var badge = await _context.Badges.FindAsync(id);
            if (badge == null) return null;

            return new BadgeDTO
            {
                Id = badge.Id,
                Name = badge.Name,
                ImageUrl = badge.ImageUrl,
                CreatedAt = badge.CreatedAt
            };
        }

        public async Task<BadgeDTO> CreateBadgeAsync(CreateBadgeDTO badgeDto)
        {
            var badge = new Badge
            {
                Name = badgeDto.Name,
                ImageUrl = badgeDto.ImageUrl
            };

            _context.Badges.Add(badge);
            await _context.SaveChangesAsync();

            return new BadgeDTO
            {
                Id = badge.Id,
                Name = badge.Name,
                ImageUrl = badge.ImageUrl,
                CreatedAt = badge.CreatedAt
            };
        }

        public async Task<BadgeDTO> UpdateBadgeAsync(Guid id, UpdateBadgeDTO badgeDto)
        {
            var badge = await _context.Badges.FindAsync(id);
            if (badge == null) return null;

            badge.Name = badgeDto.Name;
            badge.ImageUrl = badgeDto.ImageUrl;
            badge.UpdatedAt = DateTime.UtcNow;

            _context.Badges.Update(badge);
            await _context.SaveChangesAsync();

            return new BadgeDTO
            {
                Id = badge.Id,
                Name = badge.Name,
                ImageUrl = badge.ImageUrl,
                CreatedAt = badge.CreatedAt
            };
        }

        public async Task<bool> DeleteBadgeAsync(Guid id)
        {
            var badge = await _context.Badges.FindAsync(id);
            if (badge == null) return false;

            _context.Badges.Remove(badge);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}