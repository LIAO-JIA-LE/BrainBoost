using System.Data.SqlClient;
using System.Text;
using BrainBoost.Models;
using BrainBoost.Parameter;
using Dapper;

namespace BrainBoost.Services
{
    public class RaceRepository
    {
        #region 呼叫函式
        private readonly string? cnstr;

        public RaceRepository(IConfiguration configuration){
            cnstr = configuration.GetConnectionString("ConnectionStrings");
        }
        #endregion
        
        #region 新增搶答室資訊
        public void Room(RaceData raceData){
            string currentTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            // 新增搶答室資訊
            string sql = $@"INSERT INTO RaceRoom(race_name, race_date, race_function, race_public)
                            VALUES(@race_name, @race_date, @race_function, @race_public) ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql, new {race_name = raceData.room_information.race_name, race_date = currentTime, race_function = raceData.room_information.race_function, race_public = raceData.room_information.race_public});
            RoomQuestion(raceData);
        }
        #endregion

        #region 搶答室id
        public int GetRoomId(RaceData raceData){
            // 跟member_id和race_name尋找raceroom_id
            string sql = $@" SELECT raceroom_id FROM RaceRoom WHERE race_name = @race_name ";
            using var conn = new SqlConnection(cnstr);
            return conn.QueryFirstOrDefault<int>(sql, new {race_name = raceData.room_information.race_name});
        }
        #endregion

        #region 匯入搶答室問題
        public void RoomQuestion(RaceData raceData){
            int Raceroom_id = GetRoomId(raceData);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < raceData.room_question.question_id.Count; i++)
                stringBuilder.AppendLine($@"INSERT INTO Race_Question (raceroom_id, question_id) VALUES({Raceroom_id}, {raceData.room_question.question_id[i]})");
            using var conn = new SqlConnection(cnstr);
            conn.Execute(stringBuilder.ToString());
        }
        #endregion

        #region 搶答室列表
        public List<RaceRooms> GetList(){
            string sql = $@" SELECT	* FROM RaceRoom ORDER BY race_date DESC ";
            using (var conn = new SqlConnection(cnstr))
            return (List<RaceRooms>)conn.Query<RaceRooms>(sql);
        }
        #endregion

        #region 單一搶答室資料
        public RaceRooms Get(int Raceroom_id){
            string sql = $@" SELECT	* FROM RaceRoom WHERE raceroom_id = '{Raceroom_id}' AND is_delete = 0 ";
            using (var conn = new SqlConnection(cnstr))
            return conn.QueryFirstOrDefault<RaceRooms>(sql);
        }
        #endregion
    }
}
