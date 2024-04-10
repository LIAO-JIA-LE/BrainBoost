using BrainBoost.Parameter;
using BrainBoost.Services;
using Microsoft.AspNetCore.Mvc;
using BrainBoost.Models;
using BrainBoost.ViewModels;
using Microsoft.IdentityModel.Tokens;


namespace BrainBoost.Controllers
{
    [Route("BrainBoost/[controller]")]
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
        [HttpGet]
        public Question Question([FromQuery]int question_id){
            return QuestionService.GetQuestionById(question_id);
        }

        // 獲得 單一問題
        //需指定MEmBER_ID
        [HttpGet("[Action]")]
        public List<Question> All_Question([FromQuery]string search,[FromQuery]int type = 0,[FromQuery]int page = 1){
            QuestionViewModel data = new(){
                forpaging = new Forpaging(page),
            };
            data.question = QuestionService.GetQuestionList(type,search,data.forpaging);
            return data.question;
        }
        // #endregion
    }
}