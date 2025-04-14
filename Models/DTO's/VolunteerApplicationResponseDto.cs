using TSU360.Models.Enums;

public class VolunteerApplicationResponseDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public Guid EventId { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime AppliedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public List<SurveyResponseDto> Responses { get; set; }
}