using TSU360.Models.DTO_s;
using TSU360.Models.Entities;

public interface IShopService
{
    Task<IEnumerable<object>> GetAllShopsWithItemsAsync();
    Task<ShopItem> CreateShopItemAsync(CreateShopItemDTO dto);
    Task<bool> UpdateShopItemAsync(Guid id, UpdateShopItemDTO dto);
    Task<bool> DeleteShopItemAsync(Guid id);
}
