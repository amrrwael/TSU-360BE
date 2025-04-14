using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TSU360.Models.Entities
{
    public class ShopItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        [ForeignKey("Shop")]
        public Guid ShopId { get; set; }
        public Shop Shop { get; set; }
    }


}
