using BrainBoost.Models;

namespace BrainBoost.Parameter
{
    public class RaceData
    {
        // 搶答室資訊
        public RaceRooms room_information{ get; set; }

        // 搶答室的題目
        public Race_Question room_question{ get; set; }
    }
}
