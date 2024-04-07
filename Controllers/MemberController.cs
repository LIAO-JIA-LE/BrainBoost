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
        readonly RoleService RoleService;
        readonly IWebHostEnvironment evn;
        public MemberController(IWebHostEnvironment _evn,MemberService _memberservice,MailService _mailservice, JwtHelpers _jwtHelpers,Forpaging _forpaging,RoleService _roleservice){
            MemberService = _memberservice;
            MailService = _mailservice;
            JwtHelpers = _jwtHelpers;
            Forpaging = _forpaging;
            RoleService = _roleservice;
            evn = _evn;
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
                var wwwroot = evn.ContentRootPath + @"\wwwroot\images\";
                Member Member = new()
                {
                    Member_Name = RegisterData.Member_Name,
                    Member_Photo = wwwroot + "default.jpg",
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

        //修改個人資料
        [HttpPut]
        [Route("Member")]
        public IActionResult UpdateMemberData(IFormFile img,string name){
            MemberUpdate member = new()
            {
                member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id,
                member_name = name
            };
            //處理圖片
            var wwwroot = evn.ContentRootPath + @"\wwwroot\images\";
            if(img.Length > 0){
                var imgname = User.Identity.Name + ".jpg";
                var img_path = wwwroot + imgname;
                using var stream = System.IO.File.Create(img_path);
                img.CopyTo(stream);
                member.member_photo = img_path;
            }
            else{
                member.member_photo = wwwroot + "default.jpg";
            }
            MemberService.UpdateMemberData(member);
            return Ok();
        }

        // #region 無權限 & 未登入
        // // 無權限
        // [HttpGet("[Action]")]
        // public IActionResult NoAccess()
        // {
        //     return BadRequest("沒有權限");
        // }
        
        // // 無登入
        // [HttpGet("[Action]")]
        // public IActionResult NoLogin()
        // {
        //     return BadRequest("未登入");
        // }
        // #endregion

        // #region 權限
        // // 獲得驗證後名稱
        // [HttpGet("[Action]")]
        // public IActionResult GetName(){
        //     return Ok(User.Identity?.Name);
        // }
        
        // #endregion

        #region 後台管理者
        //取得目前所有使用者
        //[Authorize(Roles = "Admin")]
        [HttpGet("[Action]")]
        public MemberViewModels MemberList([FromQuery]string? Search,[FromQuery]int page = 1){
            MemberViewModels data = new(){
                forpaging = new Forpaging(page)
            };
            data.member = MemberService.GetAllMemberList(Search,data.forpaging);
            data.search = Search;
            return data;
        }
        //取得單一使用者(帳號)
        // 未來可加任課科目&上課科目
        [HttpGet("{account}")]
        public Member MemberByAcc([FromRoute]string account){
            return MemberService.GetDataByAccount(account);
        }

        #endregion

        #region 忘記密碼
        // 輸入Email後寄驗證信
        [HttpPost]
        [Route("[Action]")]
        public IActionResult ForgetPassword([FromBody] string Email)
        {
            // 看有沒有Email的資料
            Member Data = MemberService.GetDataByEmail(Email);
            // 有就寄驗證信，沒有則回傳「查無用戶」
            if (Data != null)
            {
                Data.Member_AuthCode = MailService.GenerateAuthCode();
                // 製作AuthCode
                MemberService.ChangeAuthCode(Data.Member_AuthCode, Email);
                // 寄驗證信
                var path = Directory.GetCurrentDirectory() + "/Verificationletter/ForgetPasswordTempMail.html";
                string TempMail = System.IO.File.ReadAllText(path);
                string MailBody = MailService.GetMailBody(TempMail, Data.Member_Name, Data.Member_AuthCode);
                MailService.SendMail(MailBody, Email);
                string str = "寄信成功，請收信。";
                return Ok(str);
            }
            else
                return BadRequest("查無用戶");
        }

        // 檢查驗證碼
        [HttpPost]
        [Route("[Action]")]
        public IActionResult CheckForgetPasswordCode([FromBody] CheckForgetPasswordAuthCode Data)
        {
            // 取得此Email的會員資訊
            Member Member = MemberService.GetDataByEmail(Data.Email);
            // 判斷驗證碼是否正確
            if (Member.Member_AuthCode == Data.AuthCode)
            {
                RoleService.SetMemberRole_ForgetPassword(Member.Member_Id);
                int Role = MemberService.GetRole(Member.Member_Account);
                var jwt = JwtHelpers.GenerateToken(Member.Member_Account, Role);
                var result = new
                {
                    SuccessMessage = "驗證成功",
                    StatusCode = 200,
                    Token = jwt
                };
                // 回傳成功
                return Ok(result);
            }
            else
            {
                // 回傳失敗
                return BadRequest("驗證碼錯誤");
            }
        }

        // 修改密碼
        [HttpPost]
        [Route("[Action]")]
        [Authorize(Roles = "ForgetPassword")]
        public IActionResult ChangePassword([FromBody] CheckForgetPassword Data)
        {
            // 取得此Email的會員資訊
            if (User.IsInRole("ForgetPassword"))
            {
                MemberService.ClearAuthCode(Data.Email);
                MemberService.ChangePasswordByForget(Data);
                return Ok("修改密碼成功！請再次登入！");
            }
            else
            {
                // 用戶未獲得足夠的權限
                return BadRequest("您無權執行此操作。");
            }

        }
        #endregion
    }
}
