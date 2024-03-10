namespace BrainBoost.Models
{
    public class RaceRooms
    {
        // 搶答室編號
        public int raceroom_id { get; set; }

        // 搶答室名稱
        public int race_name { get; set; }

        // 創建時間
        public DateTime race_date { get; set; }

        // 驗證碼
        public string race_code { get; set; }

        // 公開
        public bool race_public{ get; set; }
    }
}
