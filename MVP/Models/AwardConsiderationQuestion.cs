namespace MVP.Models
{
    /// <summary>
    /// Model for the MVP award consideration questions.
    /// </summary>
    public class AwardConsiderationQuestion
    {
        public string AwardQuestionId { get; set; }
        public string QuestionContent { get; set; }
        public bool IsRequired { get; set; } = true;
    }
}
