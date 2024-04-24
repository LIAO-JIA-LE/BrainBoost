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
            Response result;
            try
            {
                Member member = MemberService.GetDataByAccount(data.account);
                if(member == null){
                    return BadRequest(new Response(){
                        status_code = 400,
                        message = "無此用戶"
                    });
                }
                RoleService.UpdateMemberRole(member.Member_Id,data.role);
                result = new(){
                    status_code = 200,
                    message = "權限已成功修改為 ",
                    data = MemberService.GetDataByAccount(data.account)
                };
                int Role = MemberService.GetRole(member.Member_Account);
                if(Role == 1)
                    result.message += "Student";
                else if(Role == 2)
                    result.message += "Teacher";
                else if(Role == 3)
                    result.message += "Manager";
                else
                    result.message += "Admin";
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new Response(){
                    status_code = 400,
                    message = e.Message
                });
                
            }
        }
        #endregion
        
        #region 設定權限
        // 獲得權限
        [HttpGet("[Action]")]
        [Authorize]
        public JsonResult GetRole(){
            Response result;
            try
            {
                result = new(){
                    status_code = 200,
                    message = "權限已成功修改為 "
                };
                int Role = MemberService.GetRole(User.Identity?.Name);
                if(Role == 1)
                    result.message += "Student";
                else if(Role == 2)
                    result.message += "Teacher";
                else if(Role == 3)
                    result.message += "Manager";
                else
                    result.message += "Admin";
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
    }
}