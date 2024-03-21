namespace BrainBoost.Models
{
    public class RaceRooms
    {
        // 搶答室編號
        public int raceroom_id { get; set; }

        // 會員編號
        public int member_id{get;set;}

        // 搶答室名稱
        public string race_name { get; set; }

        // 創建時間
        public DateTime race_date { get; set; }

        // 驗證碼
        public string? race_code { get; set; }

        // 搶答室模式
        public bool race_function{get;set;} 

        // 公開
        public bool race_public{ get; set; }

        // 假刪除
        public bool is_delete{get;set;}
    }
}
