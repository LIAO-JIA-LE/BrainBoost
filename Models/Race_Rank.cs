namespace BrainBoost.Models
{
    public class Race_Rank
    {
        // 排名記錄編號
        public int rank_id { get; set; }

        // 搶答室編號
        public int raceroom_id { get; set; }

        // 會員編號
        public int member_id { get; set; }

        // 總成績
        public int total_score { get; set; }
    }
}
