using System;
using System.Data.SqlClient;
using System.Net.Mail;
using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;

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
        public void InsertRoom(InsertRoom raceData){
            string Code = GetCode();
            while(RaceRepository.GetRaceRoomByCode(Code) != null){
                Code = GetCode();
            }
            RaceRepository.InsertRoom(Code, raceData);
        }
        #endregion
        
        #region 修改搶答室（資訊和問題分開）
        // 修改 搶答室資訊（名稱、時間、公開）
        public void RoomInformation(int id, RaceInformation raceData){
            RaceRepository.RoomInformation(id, raceData);
        }
        #endregion


        // 新增 搶答室題目
        public void RoomQuestionList(int id, List<int> question_id_list){
            RaceRepository.InsertQuestion(id, question_id_list);
        }

         // 新增 搶答室題目單一
        public void RoomQuestion(int id, int question_id){
            RaceRepository.InsertQuestion(id, question_id);
        }

        // 取消選取 搶答室題目
        public void DeleteRoomQuestion(int raceroom_id, int question_id){
            RaceRepository.DeleteQuestion(raceroom_id, question_id);
        }
        // #endregion

        #region 刪除搶答室
        public void DeleteRoom(int id){
            RaceRepository.DeleteRoom(id);
        }
        #endregion  

        #region 搶答室題目列表
        public List<SimpleQuestion> RoomQuestionList(int id){
            return RaceRepository.QuestionList(id);
        }
        #endregion

        #region 搶答室題目單一
        public List<RaceQuestionAnswer> GetRoomQuestion(int id, int question_id){
            return RaceRepository.Question(id, question_id);
        }
        #endregion

        #region 隨機邀請碼
        public string GetCode()
        {
            string[] Code = {"1","2","3","4","5","6","7","8","9","0"};
            Random rd = new();
            string ValidateCode = string.Empty;
            for (int i = 0; i < 6; i++)
                ValidateCode += Code[rd.Next(Code.Length)];
            return ValidateCode;
        }
        #endregion

        #region 多重篩選
        public List<SimpleQuestion> GetSearchList(Forpaging paging, QuestionFiltering Search){
            List<SimpleQuestion> DataList = new List<SimpleQuestion>();
            if(Search.subject_id == null && Search.type_id == null && Search.tag_id == null && Search.question_level == null && Search.search == null){
                RaceRepository.SetMaxPaging(Search.member_id, paging);
                DataList = RaceRepository.GetAllQuestionList(Search.member_id, paging);
            }
            else{
                RaceRepository.SetMaxPaging(paging, Search);
                DataList = RaceRepository.GetAllQuestionList(paging, Search);
            }
            return DataList;
        }
        #endregion

        #region 刪除邀請碼
        public void DeleteCode(int id){
            RaceRepository.DeleteCode(id);
        }
        #endregion

        #region 標籤列表
        public List<Tag> TagList(int member_id, int subject_id){
            return RaceRepository.TagList(member_id);
        }
        #endregion

        #region 隨機出題
        public RaceQuestionViewModel RandomQuestion(int id){
            // 獲得題目id跟題型id
            List<RaceQuestionListType> questionIdList = RaceRepository.GetRaceRoomQuestionType(id);

            if(questionIdList.Count > 0){
                // 隨機出題
                Random rd = new();
                // if(questionIdList.type_id == 1)
                RaceQuestionViewModel question = RaceRepository.GetRandomQuestion(questionIdList[rd.Next(questionIdList.Count)]);
                return question;
            }
            else{
                //已經出完所有題目 重製題目的is_output
                RaceRepository.ResetRaceRoomQuestion(id);
                return null;
            }
        }
        #endregion

        #region 統計難度
        public List<int> Level(int raceroom_id){
            return RaceRepository.Level(raceroom_id);
        }
        #endregion

        #region 紀錄學生搶答室回答
        public void StudentReseponse(StudentReseponse studentReseponse){
            RaceRepository.StudentReseponse(studentReseponse);
        }
        #endregion
    }
}