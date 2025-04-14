using TSU360.Models.Enums;

namespace TSU360.Models.DTO_s
{
    public class AddQuestionDto
    {
        public string QuestionText { get; set; }
        public bool IsRequired { get; set; }
        public QuestionType QuestionType { get; set; }
    }
}
