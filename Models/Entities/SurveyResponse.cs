namespace TSU360.Models.Entities
{
    public class SurveyResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid QuestionId { get; set; }
        public SurveyQuestion Question { get; set; }

        public Guid ApplicationId { get; set; }
        public VolunteerApplication Application { get; set; }

        public string Answer { get; set; }
    }
}