using TSU360.Models.Enums;

namespace TSU360.Models.Entities
{
    public class VolunteerApplication
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; }
        public User User { get; set; }

        public Guid EventId { get; set; }
        public Event Event { get; set; }

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }

        public ICollection<SurveyResponse> Responses { get; set; }
    }
}