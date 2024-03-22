using Microsoft.AspNetCore.Mvc;
using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.Services;
using Microsoft.AspNetCore.Authorization;

namespace BrainBoost.Controllers
{
    [Route("BrainBoost/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Teacher, Manager, Admin")]
    public class RaceController : Controller
    {
        #region 呼叫函式
        readonly RaceService raceService;
        readonly QuestionsDBService questionService;

        public RaceController(RaceService _raceService, QuestionsDBService _questionService)
        {
            raceService = _raceService;
            questionService = _questionService;
        }
        #endregion

        #region 顯示搶答室含題目
        // 搶答室列表
        [HttpGet("[Action]")]
        public List<RaceRooms> GetRoomList(){
            return raceService.GetRoomList();
        }
        
        // 搶答室單一
        [HttpGet("[Action]")]
        public RaceRooms GetRoom([FromQuery]int id){
            return raceService.GetRoom(id);
        }
        #endregion
        
        #region 新增搶答室
        // 新增搶答室
        [HttpPost("[Action]")]
        public IActionResult Room([FromBody]RaceData raceData){
            try{
                raceService.Room(raceData);
            }
            catch(Exception e){
                return BadRequest(e.ToString());
            }
            return Ok("新增成功");
        }
        #endregion

        #region 修改搶答室

        // 修改 搶答室資訊
        [HttpPut("[Action]")]
        public IActionResult RoomInformation([FromQuery]int id, [FromBody]RaceData raceData){
            raceService.RoomInformation(id, raceData);
            return Ok();
        }

        // 新增 搶答室題目
        // [HttpPost("[Action]")]
        // public IActionResult RoomQuestion([FromBody]RaceData raceData){
        //     RaceService.RoomQuestion(raceData);
        //     return Ok();
        // }

        // // 刪除 搶答室題目
        // [HttpPost("[Action]/{Id}")]
        // public IActionResult Delete_Question_ByRoom([FromRoute]int id){
        //     RaceService.Delete_Question_ByRoom(id);
        //     return Ok();
        // }
        // #endregion

        // #region 刪除搶答室
        // [HttpDelete("[Action]/{id}")]
        // public IActionResult DeleteRoom([FromRoute]int id){
        //     RaceService.Delete_Room(id);
        //     return Ok();
        // }
        // #endregion

        // #region 顯示題目
        // [HttpGet("[Action]")]
        // public List<QuestionList> GetQuestionList([FromRoute]int Question_type, [FromRoute]string Search){
        //     var result = QuestionService.GetQuestionList(Question_type, Search);
        //     return result;
        // }
        #endregion
    }
}