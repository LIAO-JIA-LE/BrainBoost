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
        #region 呼叫函式
        private readonly QuestionsDBService QuestionService;
        private readonly RaceRepository RaceRepository;

        public RaceService(QuestionsDBService _questionService, RaceRepository _raceRepository){
            QuestionService = _questionService;
            RaceRepository = _raceRepository;
        }
        #endregion

        #region 顯示 搶答室資訊
        // 搶答室列表
        public List<RaceRooms> GetRoomList(){
            return RaceRepository.GetList();
        }
        
        // 搶答室單一（詳細資料）
        public RaceRooms GetRoom(int id){
            return RaceRepository.GetInformation(id);
        }
        #endregion

        #region 新增搶答室
        public void Room(RaceData raceData){
            RaceRepository.Room(raceData);
        }
        #endregion
        
        #region 修改搶答室（資訊和問題分開）
        // 修改 搶答室資訊（名稱、時間、公開）
        public void RoomInformation(int id, RaceRooms raceData){
            RaceRepository.RoomInformation(id, raceData);
        }
        #endregion


        // // 新增 搶答室題目
        // public void Insert_Question_ByRoom(RaceData raceData){
        //     string sql = String.Empty;
        //     // 新增題目
        //     for(int i = 0; i < raceData.room_question.question_id.Count; i++){
        //         sql = $@"INSERT INTO Race_Question(question_id, time_limit)
        //                         VALUES('{raceData.room_question.question_id[i]}', '{raceData.room_question.time_limit}') ";
        //     }
        //     using var conn = new SqlConnection(cnstr);
        //     conn.Execute(sql);
        // }

        // // 取消選取 搶答室題目
        // public void Delete_Question_ByRoom(int id){
        //     string sql = $@"DELETE FROM Race_Question WHERE racerooms_id = '{id}' ";
        //     using var conn = new SqlConnection(cnstr);
        //     conn.Execute(sql);
        // }
        // #endregion

        // #region 刪除搶答室
        // public void Delete_Room(int id){
        //     string sql = $@"DELETE FROM RaceRooms WHERE racerooms_id = '{id}' 
        //                     DELETE FROM Race_Question WHERE racerooms_id = '{id}'
        //                     DELETE FROM Race_Rank WHERE racerooms_id = '{id}'
        //                     DELETE FROM Room_Responses WHERE racerooms_id = '{id}' ";
        //     using var conn = new SqlConnection(cnstr);
        //     conn.Execute(sql);
        // }
        // #endregion  

        
        // #region 隨機邀請碼
        // public string GetCode()
        // {
        //     string[] Code = {"1","2","3","4","5","6","7","8","9","0"};
        //     Random rd = new();
        //     string AuthCode = string.Empty;
        //     for (int i = 0; i < 6; i++)
        //         AuthCode += Code[rd.Next(Code.Length)];
        //     return AuthCode;
        // }
        // #endregion
    }
}