// CreateEventDto.cs (in DTOs folder)
using System;
using System.ComponentModel.DataAnnotations;

namespace TSU360.DTOs
{
    public class CreateEventDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Location { get; set; }

        public string ImageUrl { get; set; }
    }
}