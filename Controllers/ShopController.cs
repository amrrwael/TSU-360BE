using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TSU360.Models.DTO_s;

[Route("api/[controller]")]
[ApiController]
public class ShopController : ControllerBase
{
    private readonly IShopService _service;

    public ShopController(IShopService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllShopsWithItems()
    {
        var result = await _service.GetAllShopsWithItemsAsync();
        return Ok(result);
    }

    [HttpPost("item")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateShopItem(CreateShopItemDTO dto)
    {
        var item = await _service.CreateShopItemAsync(dto);
        return Ok(item);
    }

    [HttpPut("item/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateShopItem(Guid id, UpdateShopItemDTO dto)
    {
        var updated = await _service.UpdateShopItemAsync(id, dto);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("item/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteShopItem(Guid id)
    {
        var deleted = await _service.DeleteShopItemAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
