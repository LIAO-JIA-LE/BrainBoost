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
        
        readonly MemberService MemberService;
        readonly RoleService RoleService;
        public RoleController(MemberService _memberservice,RoleService _roleservice){
            MemberService = _memberservice;
            RoleService = _roleservice;
        }

        [HttpGet("[Action]")]
        public List<MemberRoleList> MemberRoleLists(){
            return RoleService.GetMemberRoleList();
        }
        //修改使用者權限(後臺管理者)
        [HttpPut("[Action]")]
        public IActionResult UpdateMemberRole([FromBody]UpdateRole data){
            Member member = MemberService.GetDataByAccount(data.account);
            RoleService.UpdateMemberRole(member.Member_Id,data.role);
            return Ok();
        }
    }
}