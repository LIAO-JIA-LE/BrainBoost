namespace BrainBoost.Models
{
    public class Room_Responses
    {
        // 搶答室回應編號
        public int room_responses_id { get; set; }
        
        // 搶答室編號
        public int raceroom_id { get; set; }

        // 題目編號
        public int question_id { get; set; }

        // 會員編號
        public int member_id { get; set; }

        // 會員答案
        public string race_answer { get; set; }

        // 確認答案
        public bool check_correct { get; set; }

        // 回答時間
        public float race_time { get; set; }
    }
}
