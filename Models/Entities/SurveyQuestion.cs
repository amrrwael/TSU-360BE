using System;
using TSU360.Models.Enums;

namespace TSU360.Models.Entities
{
    public class SurveyQuestion
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string QuestionText { get; set; }
        public bool IsRequired { get; set; }
        public QuestionType QuestionType { get; set; } // Enum for multiple choice, text, etc.

        public Guid EventId { get; set; }
        public Event Event { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
