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
        public List<RaceRooms> GetRoomList(){
            return RaceService.GetRoomList();
        }
        
        // 搶答室單一
        [HttpGet]
        [Route("Room")]
        public RaceRooms GetRoom([FromQuery]int id){
            return RaceService.GetRoom(id);
        }
        #endregion   
        #region 新增搶答室
        // 新增搶答室
        [HttpPost]
        [Route("Room")]
        public IActionResult InsertRoom([FromBody]InsertRoom raceData){
            try{
                raceData.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                RaceService.Room(raceData);
            }
            catch(Exception e){
                return BadRequest(e.ToString());
            }
            return Ok("新增成功");
        }
        #endregion
        #region 修改搶答室
        // 修改 搶答室資訊
        [HttpPut]
        [Route("Room")]
        public IActionResult UpdateRoom([FromQuery]int id, [FromBody]RaceInformation raceData){
            RaceService.RoomInformation(id, raceData);
            return Ok("修改成功");
        }
        #endregion
        #region 刪除搶答室
        [HttpDelete]
        [Route("Room")]
        public IActionResult DeleteRoom([FromQuery]int id){
            RaceService.DeleteRoom(id);
            return Ok("刪除成功");
        }
        #endregion

        // TODO：搶答室題目
        #region 顯示搶答室題目（只有題目內容）
        // 搶答室題目列表
        [HttpGet("[Action]")]
        public List<SimpleQuestion> RoomQuestionList([FromQuery]int id){
            return RaceService.RoomQuestionList(id);
        }

        // 搶答室題目單一含內容
        [HttpGet("[Action]")]
        public List<RaceQuestionAnswer> RoomQuestion([FromQuery]int id, [FromQuery]int question_id){
            return RaceService.GetRoomQuestion(id, question_id);
        }
        #endregion        
        #region 新增搶答室題目
        // 新增 搶答室題目
        [HttpPost("[Action]")]
        public IActionResult RoomQuestion([FromQuery]int id, [FromBody]List<int> question_id_list){
            RaceService.RoomQuestionList(id, question_id_list);
            return Ok("新增成功");
        }
        #endregion        
        #region 刪除搶答室題目
        // 刪除 搶答室題目
        // [HttpDelete("[Action]")]
        [HttpDelete]
        [Route("RoomQuestion")]
        public IActionResult DeleteRoomQuestion([FromQuery]int id, [FromBody]List<int> question_id_list){
            RaceService.DeleteRoomQuestion(id, question_id_list);
            return Ok("刪除成功");
        }
        #endregion   
        #region 手動新增題目並加進去題庫（是非題）
        [HttpPost("[Action]")]
        public IActionResult TrueOrFalse([FromQuery]int id, [FromQuery]int subject_id, [FromBody]TureorFalse question){
            
            // 將題目細節儲存至QuestionList物件
            QuestionList questionList = new();

            // 題目分類
            questionList.TagData.tag_name = question.tag;

            // 題目敘述
            questionList.QuestionData = new Question(){
                type_id = 1,
                subject_id = subject_id,
                question_level = question.question_level,
                question_content = question.question_content
            };
            
            // 題目答案
            questionList.AnswerData = new Answer(){
                question_answer = question.is_answer ? "是" : "否",
                question_parse = question.parse
            };

            try
            {
                // questionList.QuestionData.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                questionList.QuestionData.member_id = 13;
                QuestionService.InsertQuestion(questionList);
                int question_id = QuestionService.GetQuestionId(questionList);
                RaceService.RoomQuestion(id, question_id);
            }
            catch (Exception e)
            {
                return BadRequest($"發生錯誤:  {e}");
            }
            return Ok("新增成功");
        }
        #endregion
        #region 手動新增題目並加進去題庫（選擇題）
        [HttpPost("[Action]")]
        public IActionResult MultipleChoice([FromQuery]int id, [FromQuery]int subject_id, [FromBody]InsertQuestion question){
            
            // 將題目細節儲存至QuestionList物件
            QuestionList questionList = new();

            // 題目分類
            questionList.TagData.tag_name = question.tag;

            // 題目敘述
            questionList.QuestionData = new Question(){
                type_id = 2,
                subject_id = subject_id,
                question_level = question.question_level,
                question_content = question.question_content
            };

            // 題目選項
            questionList.Options = new List<string>(){
                question.optionA.ToString(),
                question.optionB.ToString(),
                question.optionC.ToString(),
                question.optionD.ToString(),
            };

            // 題目答案
            questionList.AnswerData = new Answer(){
                question_answer = question.answer,
                question_parse = question.parse
            };

            try
            {
                questionList.QuestionData.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                // questionList.QuestionData.member_id = 1;
                QuestionService.InsertQuestion(questionList);
                int question_id = QuestionService.GetQuestionId(questionList);
                RaceService.RoomQuestion(id, question_id);
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
        public List<SimpleQuestion> QuestionFilterList([FromBody]QuestionFiltering SearchData, [FromQuery]int page = 1){           
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
            int member_id = 1;
            return RaceService.TagList(member_id, subject_id);
        }
        #endregion

        // TODO：統計難度
        #region level統整
        #endregion

        // TODO：搶答室開始
        #region 隨機出題
        // 單一
        [HttpGet("[Action]")]
        public RaceQuestionViewModel RandomQuestion([FromQuery]int id){
            return RaceService.RandomQuestion(id);
        }
        #endregion

        

        // 隨機亂碼
        #region 刪除隨機亂碼
        [HttpDelete("[Action]")]
        public IActionResult Code([FromQuery]int id){
            RaceService.DeleteCode(id);
            return Ok("刪除成功");
        }
        #endregion
    

    }
}