using System;
using System.Data.SqlClient;
using System.Net.Mail;
using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.ViewModels;
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
        public void Room(InsertRoom raceData){
            RaceRepository.Room(raceData);
        }
        #endregion
        
        #region 修改搶答室（資訊和問題分開）
        // 修改 搶答室資訊（名稱、時間、公開）
        public void RoomInformation(int id, RaceInformation raceData){
            RaceRepository.RoomInformation(id, raceData);
        }
        #endregion


        // 新增 搶答室題目
        public void RoomQuestion(int id, List<int> question_id_list){
            RaceRepository.InsertQuestion(id, question_id_list);
        }

        // 取消選取 搶答室題目
        public void DeleteRoomQuestion(int id, List<int> question_id_list){
            RaceRepository.DeleteQuestion(id, question_id_list);
        }
        // #endregion

        #region 刪除搶答室
        public void DeleteRoom(int id){
            RaceRepository.DeleteRoom(id);
        }
        #endregion  

        #region 搶答室題目列表
        public List<RaceQuestion> RoomQuestionList(int id){
            return RaceRepository.QuestionList(id);
        }
        #endregion

        #region 搶答室題目單一
        public List<RaceQuestionAnswer> RoomQuestion(int id, int question_id){
            return RaceRepository.Question(id, question_id);
        }
        #endregion

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

        #region 多重篩選
        public List<RaceQuestion> GetSearchList(Forpaging paging, QuestionFiltering Search){
            List<RaceQuestion> DataList = new List<RaceQuestion>();
            if(Search.subject_id == null && Search.type_id == null && Search.tag_id == null && Search.question_level == null && Search.search == null){
                RaceRepository.SetMaxPaging(paging);
                DataList = RaceRepository.GetAllQuestionList(paging);
            }
            else{
                RaceRepository.SetMaxPaging(paging, Search);
                DataList = RaceRepository.GetAllQuestionList(paging, Search);
            }
            return DataList;
        }
        #endregion
    }
}