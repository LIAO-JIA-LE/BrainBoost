namespace BrainBoost.Models
{
    public class Race_Question
    {
        // 搶答題目編號
        public int race_question_id { get; set; }

        // 搶答室編號
        public int racerooms_id { get; set; }

        // 題目編號
        public List<int> question_id { get; set; }
    }
}
