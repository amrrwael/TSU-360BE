namespace TSU360.Models.DTO_s
{
    public class ShopItemDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public Guid ShopId { get; set; }
    }

    public class CreateShopItemDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public Guid ShopId { get; set; }
    }

    public class UpdateShopItemDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }


}
