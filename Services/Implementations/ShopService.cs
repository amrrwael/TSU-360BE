using Microsoft.EntityFrameworkCore;
using TSU360.Database;
using TSU360.Models.DTO_s;
using TSU360.Models.Entities;

public class ShopService : IShopService
{
    private readonly ApplicationDbContext _context;

    public ShopService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<object>> GetAllShopsWithItemsAsync()
    {
        return await _context.Shops
            .Include(s => s.Items)
            .Select(s => new
            {
                ShopId = s.Id,
                Items = s.Items.Select(i => new
                {
                    i.Id,
                    i.Name,
                    i.ImageUrl,
                    i.Description
                })
            })
            .ToListAsync();
    }

    public async Task<ShopItem> CreateShopItemAsync(CreateShopItemDTO dto)
    {
        var item = new ShopItem
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            ShopId = dto.ShopId
        };

        _context.ShopItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> UpdateShopItemAsync(Guid id, UpdateShopItemDTO dto)
    {
        var item = await _context.ShopItems.FindAsync(id);
        if (item == null) return false;

        item.Name = dto.Name;
        item.Description = dto.Description;
        item.ImageUrl = dto.ImageUrl;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteShopItemAsync(Guid id)
    {
        var item = await _context.ShopItems.FindAsync(id);
        if (item == null) return false;

        _context.ShopItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
}
