using System.ComponentModel.DataAnnotations;

namespace TSU360.Models.Entities
{
    public class TokenBlacklist
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}
