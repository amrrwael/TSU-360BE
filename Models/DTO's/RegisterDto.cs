using TSU360.Models.Enums;
using System;

namespace TSU360.DTOs.Auth
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public Faculty Faculty { get; set; }
        public int Year { get; set; }

    }
}