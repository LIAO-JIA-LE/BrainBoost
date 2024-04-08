using Microsoft.AspNetCore.Mvc;
using BrainBoost.Services;
using BrainBoost.Models;
using Microsoft.AspNetCore.Authorization;
using BrainBoost.Parameter;
using BrainBoost.ViewModels;

namespace BrainBoost.Controllers
{
    [Route("BrainBoost/[controller]")]
    public class RoleController : Controller
    {
        #region 呼叫函式
        readonly MemberService MemberService;
        readonly RoleService RoleService;
        public RoleController(MemberService _memberservice,RoleService _roleservice){
            MemberService = _memberservice;
            RoleService = _roleservice;
        }
        #endregion
        
        #region 後台 修改使用者權限
        //修改使用者權限(後臺管理者)
        [HttpPut("[Action]")]
        public IActionResult UpdateMemberRole([FromBody]UpdateRole data){
            Member member = MemberService.GetDataByAccount(data.account);
            RoleService.UpdateMemberRole(member.Member_Id,data.role);
            return Ok();
        }
        #endregion
        
        #region 設定權限
        // 獲得權限
        [HttpGet("[Action]")]
        [Authorize]
        public IActionResult GetRole(){
            int Role = MemberService.GetRole(User.Identity?.Name);
            if(Role == 1)
                return Ok("Student");
            else if(Role == 2)
                return Ok("Teacher");
            else if(Role == 3)
                return Ok("Manager");
            else
                return Ok("Admin");
        }
        #endregion
    }
}