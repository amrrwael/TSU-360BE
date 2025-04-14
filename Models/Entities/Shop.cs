using System.ComponentModel.DataAnnotations;

namespace TSU360.Models.Entities
{
    public class Shop
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public ICollection<ShopItem> Items { get; set; }
    }

}
