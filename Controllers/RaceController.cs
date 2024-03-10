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
        #region 呼叫Service
        readonly RaceService RaceService;

        public RaceController(RaceService _RaceService)
        {
            RaceService = _RaceService;
        }
        #endregion

        // 搶答室列表
        public List<RaceRooms> RaceRoomList(){
            return RaceService.RaceRoomList();
        }
        
        // 新增搶答室
        [HttpPost("[Action]")]
        public IActionResult InsertRaceRoom([FromBody]RaceData raceData){
            RaceService.InsertRaceRoom(raceData);
            return Ok();
        }
    }
}