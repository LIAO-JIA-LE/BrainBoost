using BrainBoost.Models;

namespace BrainBoost.ViewModels
{
    public class RaceInformation
    {
        // 搶答室編號
        public int raceroom_id{get;set;}

        // 搶答室名稱
        public string race_name { get; set; }

        // 搶答室模式
        public bool race_function{get;set;} 

        // 公開
        public bool race_public{ get; set; }

        // 限時
        public int time_limit{get;set;}

    }
}
