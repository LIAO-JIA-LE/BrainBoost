using BrainBoost.Parameter;
using BrainBoost.Services;
using Microsoft.AspNetCore.Mvc;
using BrainBoost.Models;
using BrainBoost.ViewModels;
using Microsoft.IdentityModel.Tokens;


namespace BrainBoost.Controllers
{
    [Route("[controller]")]
    public class QuestionController : Controller
    {
        #region 呼叫函示
        private readonly QuestionsDBService QuestionService;
        readonly MemberService MemberService;

        public QuestionController(QuestionsDBService _questionService,MemberService _memberService)
        {
            QuestionService = _questionService;
            MemberService = _memberService;
        }
        #endregion

        // #region 顯示問題
        // 獲得 單一問題
        // [HttpPost("[Action]")]
        // public IActionResult Get_Question(int page = 1){
        //     QuestionService.Get();
        //     return Ok();
        // }

        // 獲得 單一問題
        // [HttpPost("[Action]")]
        // public IActionResult Get_QuestionList(int page = 1){
            
        //     return Ok();
        // }
        // #endregion
    }
}