using BrainBoost.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BrainBoost.Parameter;

namespace BrainBoost.Controllers;
// 登入
[Route("BrainBoost/[controller]")]
[ApiController]
[AllowAnonymous]
public class LoginController : ControllerBase
{
    #region 呼叫Service
    readonly MemberService MemberService;
    readonly JwtHelpers JwtHelpers;
    public LoginController(MemberService _MemberService,JwtHelpers _jwtHelpers){
        MemberService = _MemberService;
        JwtHelpers = _jwtHelpers;
    }
    #endregion

    // 登入
    [HttpPost]
    public IActionResult Login(MemberLogin Member)
    {
        string ValidateStr = MemberService.LoginCheck(Member.Member_Account, Member.Member_Password);
        if (!string.IsNullOrWhiteSpace(ValidateStr)){
            var result = new {
                ErrorMessage = ValidateStr,
                StatusCode = 400
            };
            return BadRequest(result);
        }
        else
        {
            int Role = MemberService.GetRole(Member.Member_Account);
            var jwt = JwtHelpers.GenerateToken(Member.Member_Account,Role);
            var result = new { 
                SuccessMessage = "登入成功",
                StatusCode = 200,
                Token = jwt 
            };
            return Ok(result);
        }
    }

    // 登出
    [HttpDelete]
    public IActionResult Logout()
    {
        return Ok("已登出");
    }
    
    // 無權限
    [HttpGet("NoAccess")]
    public IActionResult NoAccess()
    {
        return BadRequest("沒有權限");
    }
    
    // 無登入
    [HttpGet("NoLogin")]
    public IActionResult NoLogin()
    {
        return BadRequest("未登入");
    }
    
    // 獲得驗證後名稱
    [HttpGet("GetName")]
    public IActionResult GetName(){
        return Ok(User.Identity?.Name);
    }
    [HttpGet("GetRole")]
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
}
