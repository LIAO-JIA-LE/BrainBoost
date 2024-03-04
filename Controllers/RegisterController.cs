using Microsoft.AspNetCore.Mvc;
using QuestAI.Services;
using QuestAI.Models;
using Microsoft.AspNetCore.Authorization;

namespace QuestAI.Controllers
{
    [Route("QuestAI/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class RegisterController : ControllerBase
    {
        #region 呼叫Service
        readonly MemberService MemberService;
        readonly MailService MailService;
        public RegisterController(MemberService _MemberService,MailService _MailService){
            MemberService = _MemberService;
            MailService = _MailService;
        }
        #endregion
        
        // 註冊 
        [HttpPost]
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
                    Member_Name = RegisterData.Member_Username,
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
                    Path = "/QuestAI/Register/MailValidate?" + querystr
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
        [HttpGet("MailValidate")]
        public IActionResult MailValidate(string Account,string AuthCode)
        {
            if (MemberService.MailValidate(Account, AuthCode))
                return Ok("已驗證成功");
            else
                return BadRequest("請重新確認或重新註冊");
        }
    }
}
