using System;
using System.Data.SqlClient;
using System.Net.Mail;
using BrainBoost.Models;
using BrainBoost.Parameter;
using Dapper;

namespace BrainBoost.Services
{
    public class RaceService
    {
        #region 宣告連線字串
        private readonly string? cnstr;
        public RaceService(IConfiguration configuration){
            cnstr = configuration.GetConnectionString("ConnectionStrings");
        }
        #endregion

        #region 搶答室列表
        public List<RaceRooms> GetRaceRoomList(){
            string sql = $@" SELECT	* FROM RaceRooms ORDER BY race_date DESC ";
            using (var conn = new SqlConnection(cnstr))
            return new List<RaceRooms>(conn.Query<RaceRooms>(sql));
        }
        #endregion

        #region 新增搶答室
        public void InsertRaceRoom(RaceData raceData){
            // 驗證碼
            Random rd = new Random();
            string ValidateCode = GenerateAuthCodeFromRoom();

            // 新增搶答室資訊
            string sql = $@"INSERT INTO RaceRooms(race_name, race_date, race_code)
                            VALUES('{raceData.room_information.race_name}', '{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}',
                            '{ValidateCode}', '{raceData.room_information.race_public}') ";
            
            // 新增題目
            for(int i = 0; i < raceData.room_question.question_id.Count; i++){
                sql += $@"  INSERT INTO Race_Question(question_id, time_limit)
                            VALUES('{raceData.room_question.question_id[i]}', '{raceData.room_question.time_limit}') ";
            }
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql);
        }
        #region 驗證碼
        public string GenerateAuthCodeFromRoom()
        {
            string[] Code = {"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","S","T","U","V","X","Y","Z",
                            "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","s","t","u","v","x","y","z",
                            "1","2","3","4","5","6","7","8","9","0"};
            Random rd = new();
            string AuthCode = string.Empty;
            for (int i = 0; i < 6; i++)
                AuthCode += Code[rd.Next(Code.Length)];
            return AuthCode;
        }
        #endregion
        #endregion

        #region 修改搶答室
        // public void UpdateRace
        #endregion
    }
}