using Microsoft.AspNetCore.Mvc;
using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.Services;
using Microsoft.AspNetCore.Authorization;

namespace BrainBoost.Controllers
{
    [Route("BrainBoost/[controller]")]
    [ApiController]
    [Authorize(Roles = "Teacher, Manager, Admin")]
    public class RaceController : Controller
    {
        #region 呼叫函式
        readonly RaceService RaceService;
        readonly QuestionsDBService QuestionService;

        public RaceController(RaceService _RaceService, QuestionsDBService _questionService)
        {
            RaceService = _RaceService;
            QuestionService = _questionService;
        }
        #endregion

        #region 顯示搶答室
        // 搶答室列表
        [HttpGet("[Action]")]
        public List<RaceRooms> GetRoomList(){
            return RaceService.GetRoomList();
        }
        
        // 搶答室單一
        [HttpGet("[Action]/{id}")]
        public RaceRooms GetRoom([FromRoute]int id){
            var result = RaceService.GetRoom(id);
            if (result is null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return result;
        }
        #endregion
        
        #region 新增搶答室
        // 新增搶答室
        [HttpPost("[Action]")]
        public IActionResult InsertRoom([FromBody]RaceData raceData){
            RaceService.Insert_Room(raceData);
            return Ok();
        }
        #endregion

        #region 修改搶答室
        [HttpPost("[Action]")]
        public IActionResult UpdateRoom([FromRoute]int id, [FromBody]RaceData raceData){
            RaceService.Update_Room(raceData);
            return Ok();
        }
        #endregion

        #region 刪除搶答室
        [HttpDelete("[Action]/{id}")]
        public IActionResult DeleteRaceRoom([FromRoute]int id){
            RaceService.Delete_RaceRoom(id);
            return Ok();
        }
        #endregion

        #region 顯示題目
        [HttpGet("[Action]")]
        public List<QuestionList> GetQuestionList([FromRoute]int Question_type, [FromRoute]string Search){
            var result = QuestionService.GetQuestionList(Question_type, Search);
            return result;
        }
        #endregion
    }
}