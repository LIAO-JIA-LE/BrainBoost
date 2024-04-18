using Microsoft.AspNetCore.Mvc;
using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.Services;
using BrainBoost.ViewModels;
using NPOI.SS.Formula.Functions;

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
        public IActionResult GetRoomList(){
            try
            {
                var Response = RaceService.GetRoomList();
                if(Response == null){
                    return Ok(new ResponseViewModel{
                        status_code = 200,
                        message = "查無資料",
                        data = null
                    });
                }
                else{
                    return Ok(new ResponseViewModel<List<RaceRooms>>{
                        status_code = 200,
                        message = "讀取成功",
                        data = Response
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel{
                        status_code = 400,
                        message = e.Message
                });
            }
        }
        
        // 搶答室單一
        [HttpGet]
        [Route("Room")]
        public IActionResult GetRoom([FromQuery]int raceroom_id){
            try
            {
                var Response = RaceService.GetRoom(raceroom_id);
                if(Response == null){
                    return Ok(new ResponseViewModel{
                        status_code = 200,
                        message = "查無資料",
                        data = null
                    });
                }
                else{
                    return Ok(new ResponseViewModel<RaceRooms>{
                        status_code = 200,
                        message = "讀取成功",
                        data = Response
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel{
                        status_code = 400,
                        message = e.Message
                });
            }
        }
        #endregion   
        #region 新增搶答室
        // 新增搶答室
        [HttpPost]
        [Route("Room")]
        public IActionResult InsertRoom([FromBody]InsertRoom raceData){
            try{
                raceData.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                var Response = raceData;
                RaceService.InsertRoom(raceData);
                if(Response == null){
                    return Ok(new ResponseViewModel{
                        status_code = 200,
                        message = "查無資料"
                    });
                }
                else{
                    return Ok(new ResponseViewModel<InsertRoom>{
                        status_code = 200,
                        message = "讀取成功",
                        data = Response
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel{
                        status_code = 400,
                        message = e.Message
                    });
            }
        }
        #endregion
        #region 修改搶答室
        // 修改 搶答室資訊
        [HttpPut]
        [Route("Room")]
        public IActionResult UpdateRoom([FromQuery]int raceroom_id, [FromBody]RaceInformation raceData){
            try
            {
                RaceService.RoomInformation(raceroom_id, raceData);
                var Response = RaceService.GetRoom(raceroom_id);
                return Ok(new ResponseViewModel<RaceRooms>{
                    status_code = 200,
                    message = "讀取成功",
                    data = Response
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel{
                        status_code = 400,
                        message = e.Message
                    });
            }
        }
        #endregion
        #region 刪除搶答室
        [HttpDelete]
        [Route("Room")]
        public IActionResult DeleteRoom([FromQuery]int raceroom_id){
            try
            {
                RaceService.DeleteRoom(raceroom_id);
                return Ok(new ResponseViewModel<int>{
                    status_code = 200,
                    message = "刪除成功",
                    data = raceroom_id
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel{
                        status_code = 400,
                        message = e.Message
                    });
            }
        }
        #endregion

        // TODO：搶答室題目
        #region 顯示搶答室題目（只有題目內容）
        // 搶答室題目列表
        [HttpGet("[Action]")]
        public IActionResult RoomQuestionList([FromQuery]int raceroom_id){
            try
            {
                var Response = RaceService.RoomQuestionList(raceroom_id);
                if(Response == null){
                    return Ok(new ResponseViewModel{
                        status_code = 200,
                        message = "查無資料"
                    });
                }
                else{
                    return Ok(new ResponseViewModel<List<SimpleQuestion>>{
                        status_code = 200,
                        message = "顯示成功",
                        data = Response
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel{
                        status_code = 400,
                        message = e.Message
                    });
            }
        }

        // 搶答室題目單一含內容
        [HttpGet("[Action]")]
        public IActionResult RoomQuestion([FromQuery]int raceroom_id, [FromBody]int question_id){
            try
            {
                var Response = RaceService.GetRoomQuestion(raceroom_id, question_id);
                if(Response == null){
                    return Ok(new ResponseViewModel{
                        status_code = 200,
                        message = "查無資料"
                    });
                }
                else{
                    return Ok(new ResponseViewModel<List<RaceQuestionAnswer>>{
                        status_code = 200,
                        message = "顯示成功",
                        data = Response
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel{
                        status_code = 400,
                        message = e.Message
                    });
            }
        }
        #endregion        
        #region 新增搶答室題目
        // 新增 搶答室題目
        [HttpPost("[Action]")]
        public IActionResult RoomQuestion([FromQuery]int raceroom_id, [FromBody]RoomQuestionList roomQuestionList){
            try{
                var Response = roomQuestionList;
                RaceService.RoomQuestionList(raceroom_id, roomQuestionList);
                return Ok(new ResponseViewModel<RoomQuestionList>{
                        status_code = 200,
                        message = "顯示成功",
                        data = Response
                    });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel{
                        status_code = 400,
                        message = e.Message
                    });
            }
        }
        #endregion        
        #region 刪除搶答室題目
        // 刪除 搶答室題目
        [HttpDelete]
        [Route("RoomQuestion")]
        public IActionResult DeleteRoomQuestion([FromQuery]int raceroom_id, [FromBody]RoomQuestionList roomQuestionList){
            try{
                var Response = roomQuestionList;
                RaceService.RoomQuestionList(raceroom_id, roomQuestionList);
                return Ok(new ResponseViewModel<RoomQuestionList>{
                        status_code = 200,
                        message = "顯示成功",
                        data = Response
                    });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel{
                        status_code = 400,
                        message = e.Message
                    });
            }
        }
        #endregion   
        #region 手動新增題目並加進去題庫（是非題）
        [HttpPost("[Action]")]
        public IActionResult TrueOrFalse([FromQuery]int raceroom_id, [FromQuery]int subject_id, [FromBody]TureorFalse question){
            try
            {
                // 將題目細節儲存至QuestionList物件
                QuestionList questionList = new();
                // 回應物件
                Response result; 
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
                questionList.QuestionData.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                QuestionService.InsertQuestion(questionList);
                int question_id = QuestionService.GetQuestionId(questionList);
                RaceService.RoomQuestion(raceroom_id, question_id);

                var Response = question;
                return Ok(new ResponseViewModel<TureorFalse>{
                    status_code = 200,
                    message = "新增是非題成功",
                    data = Response
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel{
                        status_code = 400,
                        message = e.Message
                    });
            }
        }
        #endregion
        #region 手動新增題目並加進去題庫（選擇題）
        [HttpPost("[Action]")]
        public IActionResult MultipleChoice([FromQuery]int raceroom_id, [FromQuery]int subject_id, [FromBody]MultipleChoice question){
            try
            {
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

                questionList.QuestionData.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                // questionList.QuestionData.member_id = 1;
                QuestionService.InsertQuestion(questionList);
                int question_id = QuestionService.GetQuestionId(questionList);
                RaceService.RoomQuestion(raceroom_id, question_id);
                var Response = question;
                return Ok(new ResponseViewModel<MultipleChoice>{
                    status_code = 200,
                    message = "新增是非題成功",
                    data = Response
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel{
                        status_code = 400,
                        message = e.Message
                    });
            }
        }
        #endregion+

        // TODO：題庫（多重篩選）
        #region 題庫列表
        [HttpGet("[Action]")]
        public IActionResult QuestionFilterList([FromQuery]QuestionFiltering SearchData, [FromQuery]int page = 1){           
            try{
                QuestionFiltering Data = new QuestionFiltering(){
                    subject_id = SearchData.subject_id,
                    member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id,
                    tag_id = SearchData.tag_id,
                    question_level = SearchData.question_level,
                    search = SearchData.search
                };
                Forpaging paging = new(page);

                var Response = RaceService.GetSearchList(paging, Data);
                return Ok(new ResponseViewModel<List<SimpleQuestion>>{
                    status_code = 200,
                    message = "顯示成功",
                    data = Response
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel{
                        status_code = 400,
                        message = e.Message
                    });
            }
        }
        #endregion
    
        // TODO：篩選下拉式選單
        // #region 標籤列表
        // [HttpGet("[Action]")]
        // public IActionResult TagList([FromQuery]int subject_id){
        //     try{
        //         int member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
        //         return Ok(new ResponseViewModel<List<Tag>>{
        //             status_code = 200,
        //             message = "顯示成功",
        //             data = RaceService.TagList(member_id, subject_id)
        //         });
        //     }
        //     catch (Exception e)
        //     {
        //         return BadRequest(new ResponseViewModel{
        //                 status_code = 400,
        //                 message = e.Message
        //             });
        //     }
        // }
        // #endregion

        // TODO：統計難度
        #region level統整
        [HttpGet("[Action]")]
        public IActionResult LevelStatistics([FromQuery]int raceroom_id){
            try{
                var Response = RaceService.Level(raceroom_id);
                return Ok(new ResponseViewModel<List<int>>{
                    status_code = 200,
                    message = "顯示成功",
                    data = Response
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel{
                        status_code = 400,
                        message = e.Message
                    });
            }
        }
        #endregion

        // TODO：搶答室開始
        #region 隨機出題
        // 單一
        [HttpGet("[Action]")]
        public IActionResult RandomQuestion([FromQuery]int raceroom_id){
            try{
                var Response = RaceService.RandomQuestion(raceroom_id);
                return Ok(new ResponseViewModel<RaceQuestionViewModel>{
                    status_code = 200,
                    message = "顯示成功",
                    data = Response
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel{
                        status_code = 400,
                        message = e.Message
                    });
            }
        }
        #endregion

        #region 紀錄學生搶答室答案和分數
        [HttpPost]
        [Route("[Action]")]
        public IActionResult StudentResponse([FromQuery]int raceroom_id, [FromQuery]int question_id, [FromBody]StudentResponse studentResponse){
            
            Response result = new();
            
            try{
                // 取得member_id
                studentResponse.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                // 新增學生答案
                RaceService.StudentResponse(raceroom_id, question_id, studentResponse);
                // 確定答案
                RaceService.CheckAnswer(raceroom_id, question_id, studentResponse);
                // // 紀錄分數
                // RaceService.RecordScore(studentResponse);

                return Ok(new ResponseViewModel<StudentResponse>{
                    status_code = 200,
                    message = "紀錄成功",
                    data = studentResponse
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResponseViewModel{
                        status_code = 400,
                        message = e.Message
                    });
            }
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