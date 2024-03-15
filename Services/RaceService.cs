using System;
using System.Data.SqlClient;
using System.Net.Mail;
using BrainBoost.Models;
using BrainBoost.Parameter;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace BrainBoost.Services
{
    public class RaceService
    {
        #region 宣告連線字串
        private readonly string? cnstr;
        private readonly QuestionsDBService QuestionService;

        public RaceService(IConfiguration configuration, QuestionsDBService _questionService){
            cnstr = configuration.GetConnectionString("ConnectionStrings");
            QuestionService = _questionService;
        }
        #endregion

        #region 顯示 搶答室資訊
        // 搶答室列表
        public List<RaceRooms> GetRoomList(){
            string sql = $@" SELECT	* FROM RaceRooms ORDER BY race_date DESC ";
            using (var conn = new SqlConnection(cnstr))
            return new List<RaceRooms>(conn.Query<RaceRooms>(sql));
        }
        
        // 搶答室單一（詳細資料）
        public RaceRooms GetRoom(int id){
            string sql = $@" SELECT	* FROM RaceRooms WHERE raceroom_id = '{id}' ";
            using (var conn = new SqlConnection(cnstr))
            return conn.QueryFirstOrDefault<RaceRooms>(sql);
        }
        #endregion

        #region 新增搶答室
        public void Insert_Room(RaceData raceData){
            // 驗證碼
            string ValidateCode = GetCode();

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
        #endregion
        
        #region 隨機邀請碼
        public string GetCode()
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

        #region 修改搶答室（資訊和問題分開）
        // 修改 搶答室資訊（名稱、時間、公開）
        public void Update_Information_ByRoom(int id, RaceData raceData){
            RaceRooms Room = GetRoom(id);
            // 新增搶答室資訊
            string sql = $@"UPDATE RaceRooms SET race_name = '{raceData.room_information.race_name}',
                            race_date = '{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}' WHERE racerooms_id = '{Room.raceroom_id}'";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql);
        }

        // 新增 搶答室題目
        public void Insert_Question_ByRoom(RaceData raceData){
            string sql = String.Empty;
            // 新增題目
            for(int i = 0; i < raceData.room_question.question_id.Count; i++){
                sql = $@"INSERT INTO Race_Question(question_id, time_limit)
                                VALUES('{raceData.room_question.question_id[i]}', '{raceData.room_question.time_limit}') ";
            }
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql);
        }

        // 取消選取 搶答室題目
        public void Delete_Question_ByRoom(int id){
            string sql = $@"DELETE FROM Race_Question WHERE racerooms_id = '{id}' ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql);
        }
        #endregion

        #region 刪除搶答室
        public void Delete_Room(int id){
            string sql = $@"DELETE FROM RaceRooms WHERE racerooms_id = '{id}' 
                            DELETE FROM Race_Question WHERE racerooms_id = '{id}' ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql);
        }
        #endregion  
    }
}