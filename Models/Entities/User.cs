using Microsoft.AspNetCore.Identity;
using TSU360.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSU360.Models.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public Faculty Faculty { get; set; }
        public int Year { get; set; }

        [Column(TypeName = "nvarchar(24)")] // Explicitly define column type
        public UserRole UserRole { get; set; } = UserRole.Attendee;
    }
}