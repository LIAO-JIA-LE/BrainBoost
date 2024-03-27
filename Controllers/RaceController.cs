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

        // 搶答室資訊
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

        // 搶答室題目
        #region 顯示搶答室題目（只有題目內容）
        // 搶答室題目列表
        [HttpGet("[Action]")]
        public List<SimpleQuestion> RoomQuestionList([FromQuery]int id){
            return RaceService.RoomQuestionList(id);
        }

        // 搶答室題目單一含內容
        [HttpGet("[Action]")]
        public List<RaceQuestionAnswer> RoomQuestion([FromQuery]int id, [FromQuery]int question_id){
            return RaceService.RoomQuestion(id, question_id);
        }
        #endregion
        
        #region 新增搶答室題目
        // 新增 搶答室題目
        [HttpPost("[Action]")]
        public IActionResult RoomQuestion([FromQuery]int id, [FromBody]List<int> question_id_list){
            RaceService.RoomQuestion(id, question_id_list);
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
    
        // 題庫（多重篩選）
        #region 題庫列表
        [HttpPost("[Action]")]
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
    
        // 篩選列表
        #region 標籤列表
        [HttpGet("[Action]")]
        public List<Tag> TagList([FromQuery]int subject_id){
            // int member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
            int member_id = 1;
            return RaceService.TagList(member_id, subject_id);
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