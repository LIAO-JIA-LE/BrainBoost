using BrainBoost.Models;

namespace BrainBoost.Parameter
{
    public class QuestionList
    {
        public Question QuestionData { get; set; } = new();
        
        public List<string> Options { get; set; }

        public Answer AnswerData { get; set; } = new();
    }
}
