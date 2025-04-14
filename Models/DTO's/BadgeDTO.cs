namespace TSU360.Models.DTOs
{
    public class BadgeDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateBadgeDTO
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }

    public class UpdateBadgeDTO
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}