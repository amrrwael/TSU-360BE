using TSU360.Models.Enums;

namespace TSU360.DTOs.Auth
{
    public class UserProfileDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public String Faculty { get; set; }
       
        public int Year { get; set; }
        public string Degree { get; set; }

    }
}