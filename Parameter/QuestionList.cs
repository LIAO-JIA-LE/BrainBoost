using QuestAI.Models;

namespace QuestAI.Parameter
{
    public class QuestionList
    {
        public Question QuestionData { get; set; }
        //public Option OptionData { get; set; }
        public List<string> Options { get; set; }
        // public string? Option { get; set; }
        public Answer AnswerData { get; set; }
    }
}
