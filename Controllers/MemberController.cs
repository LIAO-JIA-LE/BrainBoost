using Microsoft.AspNetCore.Mvc;
using BrainBoost.Services;
using BrainBoost.Models;
using Microsoft.AspNetCore.Authorization;
using BrainBoost.Parameter;
using BrainBoost.ViewModels;

namespace BrainBoost.Controllers
{
    [Route("BrainBoost/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class MemberController : ControllerBase
    {
        #region 呼叫函式
        readonly MemberService MemberService;
        readonly MailService MailService;
        readonly JwtHelpers JwtHelpers;
        readonly Forpaging Forpaging;
        public MemberController(MemberService _memberservice,MailService _mailservice, JwtHelpers _jwtHelpers,Forpaging _forpaging){
            MemberService = _memberservice;
            MailService = _mailservice;
            JwtHelpers = _jwtHelpers;
            Forpaging = _forpaging;
        }
        #endregion
        

        #region 註冊
        // 註冊 
        [HttpPost("[Action]")]
        public IActionResult Register([FromBody]MemberRegister RegisterData)
        {
            if (ModelState.IsValid)
            {
                var validatestr = MemberService.RegisterCheck(RegisterData.Member_Account,RegisterData.Member_Email);
                if (!string.IsNullOrEmpty(validatestr)){
                    var result = new {
                        ErrorMessage = validatestr,
                        StatusCode = 400
                    };
                    return BadRequest(result);
                }
                Member Member = new()
                {
                    Member_Name = RegisterData.Member_Name,
                    Member_Account = RegisterData.Member_Account,
                    Member_Email = RegisterData.Member_Email,
                    Member_Password = MemberService.HashPassword(RegisterData.Member_Password),
                    Member_AuthCode = MailService.GenerateAuthCode()
                };

                var path = Directory.GetCurrentDirectory() + "/Verificationletter/RegisterTempMail.html";
                string TempMail = System.IO.File.ReadAllText(path);

                var querystr =  $@"Account={Member.Member_Account}&AuthCode={Member.Member_AuthCode}";

                var request = HttpContext.Request;
                UriBuilder ValidateUrl = new()
                {
                    Scheme = request.Scheme, // 使用請求的協議 (http/https)
                    Host = request.Host.Host, // 使用請求的主機名
                    Port = request.Host.Port ?? 80, // 使用請求的端口，如果未指定則默認使用80
                    Path = "/BrainBoost/Member/MailValidate?" + querystr
                };
                string finalUrl = ValidateUrl.ToString().Replace("%3F","?");

                string MailBody = MailService.GetMailBody(TempMail, Member.Member_Name, finalUrl);
                MailService.SendMail(MailBody, Member.Member_Email);
                string str = "寄信成功，請收信。";
                
                MemberService.Register(Member);
                return Ok(str);
            }
            return BadRequest();
        }
        
        // 郵件驗證
        [HttpGet("[Action]")]
        public IActionResult MailValidate(string Account,string AuthCode)
        {
            if (MemberService.MailValidate(Account, AuthCode))
                return Ok("已驗證成功");
            else
                return BadRequest("請重新確認或重新註冊");
        }
    
        #endregion
    
        #region 登入
        // 登入
        [HttpPost("[Action]")]
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
        #endregion

        #region 無權限 & 未登入
        // 無權限
        [HttpGet("[Action]")]
        public IActionResult NoAccess()
        {
            return BadRequest("沒有權限");
        }
        
        // 無登入
        [HttpGet("[Action]")]
        public IActionResult NoLogin()
        {
            return BadRequest("未登入");
        }
        #endregion

        #region 權限
        // 獲得驗證後名稱
        [HttpGet("[Action]")]
        public IActionResult GetName(){
            return Ok(User.Identity?.Name);
        }
        
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

        //取得目前所有使用者
        // [Authorize(Roles = "Admin")]
        [HttpGet("[Action]")]
        public MemberViewModels GetMemberList([FromQuery]string? Search,[FromQuery]int page = 1){
            MemberViewModels data = new(){
                forpaging = new Forpaging(page)
            };
            data.member = MemberService.GetAllMemberList(Search,data.forpaging);
            data.search = Search;
            return data;
        }
        //取得單一使用者(帳號)
        [HttpGet("[Action]")]
        public Member GetMemberByAcc([FromQuery]string account){
            return MemberService.GetDataByAccount(account);
        }
        //修改使用者權限(後臺管理者)
        [HttpPut("[Action]")]
        public IActionResult UpdateMemberRole([FromBody]string account, [FromBody]int role){
            Member data = MemberService.GetDataByAccount(account);
            MemberService.UpdateMemberRole(data.Member_Id,role);
            return Ok();
        }
    }
}
