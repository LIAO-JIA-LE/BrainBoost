using BrainBoost.Models;

namespace BrainBoost.Parameter
{
    public class QuestionList
    {
        public Question QuestionData { get; set; } = new();
        //public Option OptionData { get; set; }
        public List<string> Options { get; set; }
        // public string? Option { get; set; }
        public Answer AnswerData { get; set; } = new();
    }
}
