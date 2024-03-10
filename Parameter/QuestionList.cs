using BrainBoost.Models;

namespace BrainBoost.Parameter
{
    public class QuestionList
    {
        // 題目內容
        public Question QuestionData { get; set; } = new();
        
        // 選項
        public List<string> Options { get; set; }

        // 答案
        public Answer AnswerData { get; set; } = new();
    }
}
