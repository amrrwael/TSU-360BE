public class SurveyResponseDto
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } // Added for better UX
    public string Answer { get; set; }
}