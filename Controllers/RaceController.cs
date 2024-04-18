using Microsoft.AspNetCore.Mvc;
using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.Services;
using BrainBoost.ViewModels;

namespace BrainBoost.Controllers
{
    [Route("BrainBoost/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Teacher, Manager, Admin")]
    public class RaceController : Controller
    {
        #region 呼叫函式
        readonly RaceService RaceService;
        readonly QuestionsDBService QuestionService;
        readonly MemberService MemberService;

        public RaceController(RaceService _raceService, QuestionsDBService _questionService,MemberService _memberService)
        {
            RaceService = _raceService;
            QuestionService = _questionService;
            MemberService = _memberService;
        }
        #endregion

        // TODO：搶答室資訊
        #region 顯示搶答室資訊
        // 搶答室列表
        [HttpGet]
        [Route("AllRoom")]
        public JsonResult GetRoomList(){
            Response result;
            try
            {
                result = new(){
                    status_code = 200,
                    message = "讀取成功",
                    data = RaceService.GetRoomList()
                };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return new(result);
        }
        
        // 搶答室單一
        [HttpGet]
        [Route("Room")]
        public JsonResult GetRoom([FromQuery]int raceroom_id){
            Response result;
            try
            {
                result = new(){
                    status_code = 200,
                    message = "讀取成功",
                    data = RaceService.GetRoom(raceroom_id)
                };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return new(result);
        }
        #endregion   
        #region 新增搶答室
        // 新增搶答室
        // 解題方式(race_function)預設為0(0:最後一次顯示,1:逐題解析)
        // 是否公開(race_public)預設為0(0:不公開,1:公開)
        // 時間限制(time_limit)預設為30秒
        [HttpPost]
        [Route("Room")]
        public JsonResult InsertRoom([FromBody]InsertRoom raceData){
            Response result ;
            try{
                raceData.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                int raceroom_id = RaceService.InsertRoom(raceData);
                result = new(){
                    status_code = 200,
                    message = "新增成功",
                    data = RaceService.GetRoom(raceroom_id)
                };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return new(result);
        }
        #endregion
        #region 修改搶答室
        // 修改 搶答室資訊
        [HttpPut]
        [Route("Room")]
        public JsonResult UpdateRoom([FromBody]RaceInformation raceData){
            Response result;
            try
            {
                RaceService.RoomInformation(raceData);
                result = new(){
                    status_code = 200,
                    message = "修改成功"
                };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return new(result);
        }
        #endregion
        #region 刪除搶答室
        [HttpDelete]
        [Route("Room")]
        public JsonResult DeleteRoom([FromBody]int raceroom_id){
            Response result;
            try
            {
                RaceService.DeleteRoom(raceroom_id);
                result = new(){
                    status_code = 200,
                    message = "刪除成功"
                };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return new(result);
        }
        #endregion

        // TODO：搶答室題目
        #region 顯示搶答室題目（只有題目內容）
        // 搶答室題目列表
        [HttpGet("[Action]")]
        public JsonResult RoomQuestionList([FromQuery]int raceroom_id){
            Response result;
            try
            {
                result = new(){
                    status_code = 200,
                    message = "讀取成功",
                    data = RaceService.RoomQuestionList(raceroom_id)
                };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return new(result);
        }

        // 搶答室題目單一含內容
        [HttpGet("[Action]")]
        public JsonResult RoomQuestion([FromQuery]RaceroomQuestion raceroomQuestion){
            Response result;
            try
            {
                result = new(){
                    status_code = 200,
                    message = "讀取成功",
                    data = RaceService.GetRoomQuestion(raceroomQuestion)
                };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return new(result);
        }
        #endregion        
        #region 新增搶答室題目
        // 新增 搶答室題目
        [HttpPost("[Action]")]
        public JsonResult RoomQuestion([FromBody]RoomQuestionList roomQuestionList){
            Response result ;
            try{
                RaceService.RoomQuestionList(roomQuestionList);
                result = new(){
                    status_code = 200,
                    message = "新增成功",
                };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return new(result);
        }
        #endregion        
        #region 刪除搶答室題目
        // 刪除 搶答室題目
        // [HttpDelete("[Action]")]
        [HttpDelete]
        [Route("RoomQuestion")]
        public JsonResult DeleteRoomQuestion([FromBody]RaceroomQuestion raceroomQuestion){
            Response result;
            try
            {
                RaceService.DeleteRoomQuestion(raceroomQuestion);
                result = new(){
                    status_code = 200,
                    message = "刪除成功"
                };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return new(result);
        }
        #endregion   
        #region 手動新增題目並加進去題庫（是非題）
        [HttpPost("[Action]")]
        public JsonResult TrueOrFalse([FromBody]RaceroomTrueOrFalse question){
            // 將題目細節儲存至QuestionList物件
            QuestionList questionList = new();
            // 回應物件
            Response result; 
            // 題目分類
            questionList.TagData.tag_name = question.TureorFalse.tag;

            // 題目敘述
            questionList.QuestionData = new Question(){
                type_id = 1,
                subject_id = question.subject_id,
                question_level = question.TureorFalse.question_level,
                question_content = question.TureorFalse.question_content
            };
            
            // 題目答案
            questionList.AnswerData = new Answer(){
                question_answer = question.TureorFalse.is_answer ? "是" : "否",
                question_parse = question.TureorFalse.parse
            };

            try
            {
                questionList.QuestionData.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                // questionList.QuestionData.member_id = 13;
                QuestionService.InsertQuestion(questionList);
                int question_id = QuestionService.GetQuestionId(questionList);
                RaceService.RoomQuestion(question.raceroom_id, question_id);
                result = new(){
                    status_code = 200,
                    message = "新增成功"
                };
            }
            catch (Exception e)
            {
                result = new(){
                    status_code = 400,
                    message = e.Message
                };
            }
            return new(result);
        }
        #endregion
        #region 手動新增題目並加進去題庫（選擇題）
        [HttpPost("[Action]")]
        public IActionResult MultipleChoice([FromBody]RaceroomMultipleChoice question){
            
            // 將題目細節儲存至QuestionList物件
            QuestionList questionList = new();

            // 題目分類
            questionList.TagData.tag_name = question.multipleChoice.tag;

            // 題目敘述
            questionList.QuestionData = new Question(){
                type_id = 2,
                subject_id = question.subject_id,
                question_level = question.multipleChoice.question_level,
                question_content = question.multipleChoice.question_content
            };

            // 題目選項
            questionList.Options = new List<string>(){
                question.multipleChoice.optionA.ToString(),
                question.multipleChoice.optionB.ToString(),
                question.multipleChoice.optionC.ToString(),
                question.multipleChoice.optionD.ToString(),
            };

            // 題目答案
            questionList.AnswerData = new Answer(){
                question_answer = question.multipleChoice.answer,
                question_parse = question.multipleChoice.parse
            };

            try
            {
                questionList.QuestionData.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                // questionList.QuestionData.member_id = 1;
                QuestionService.InsertQuestion(questionList);
                int question_id = QuestionService.GetQuestionId(questionList);
                RaceService.RoomQuestion(question.raceroom_id, question_id);
            }
            catch (Exception e)
            {
                return BadRequest($"發生錯誤:  {e}");
            }
            return Ok("新增成功");
        }
        #endregion+

        // TODO：題庫（多重篩選）
        #region 題庫列表
        [HttpGet("[Action]")]
        public List<SimpleQuestion> QuestionFilterList([FromQuery]QuestionFiltering SearchData, [FromQuery]int page = 1){           
            QuestionFiltering Data = new QuestionFiltering(){
                subject_id = SearchData.subject_id,
                member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id,
                tag_id = SearchData.tag_id,
                question_level = SearchData.question_level,
                search = SearchData.search
            };
            Forpaging paging = new(page);
            return RaceService.GetSearchList(paging, Data);
        }
        #endregion
    
        // TODO：篩選下拉式選單
        #region 標籤列表
        [HttpGet("[Action]")]
        public List<Tag> TagList([FromQuery]int subject_id){
            // int member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
            int member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
            return RaceService.TagList(member_id, subject_id);
        }
        #endregion

        // TODO：統計難度
        #region level統整
        [HttpGet("[Action]")]
        public List<int> LevelStatistics([FromQuery]int raceroom_id){
            List<int> LevelQuestion = RaceService.Level(raceroom_id);
            return LevelQuestion;
        }
        #endregion

        // TODO：搶答室開始
        #region 隨機出題
        // 單一
        [HttpGet("[Action]")]
        public RaceQuestionViewModel RandomQuestion([FromQuery]int raceroom_id){
            RaceQuestionViewModel Data = RaceService.RandomQuestion(raceroom_id);
            return Data ?? null;
        }
        #endregion

        #region 紀錄學生搶答室答案和分數
        [HttpPost]
        [Route("[Action]")]
        public JsonResult StudentResponse([FromBody]StudentResponse studentResponse){
            
            Response result = new();
            
            try{
                // 取得member_id
                studentResponse.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                // 新增學生答案
                RaceService.StudentResponse(studentResponse);
                // 確定答案
                RaceService.CheckAnswer(studentResponse);
                // // 紀錄分數
                // RaceService.RecordScore(studentResponse);

                result = new(){
                    status_code = 200,
                    message = "匯入成功",
                    data = studentResponse
                };
            }
            catch(Exception e){
                result = new(){
                    status_code = 500,
                    message = e.Message.ToString()
                };
            }
            return new(result);
        }
        #endregion
        

        // 隨機亂碼
        // #region 刪除隨機亂碼
        // [HttpDelete("[Action]")]
        // public IActionResult Code([FromQuery]int id){
        //     RaceService.DeleteCode(id);
        //     return Ok("刪除成功");
        // }
        // #endregion
    

    }
}