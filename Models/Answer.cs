namespace BrainBoost.Models
{
    public class Answer
    {
        // 答案編號
        public int answer_id { get; set; }

        // 題目編號
        public int question_id { get; set; }

        // 答案內容
        public string question_answer { get; set; }

        // 答案解析
        public string question_parse { get; set; }
    }
}
