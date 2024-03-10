namespace BrainBoost.Models
{
    public class Option
    {
        // 選項編號
        public int option_id { get; set; }

        // 題目id
        public int question_id { get; set; }

        // 選項內容
        public string option_content { get; set; }

        // 選項照片
        public string option_picture { get; set; }

        // 是否為答案
        public bool is_answer { get; set; }
    }
}
