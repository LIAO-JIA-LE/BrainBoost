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
        [HttpGet]
        [Route("AllQuestion")]
        public IActionResult All_Question([FromQuery]string search,[FromQuery]int type = 0,[FromQuery]int page = 1){
            QuestionViewModel data = new(){
                forpaging = new Forpaging(page),
            };
            int member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
            data.question = QuestionService.GetQuestionList(member_id,type,search,data.forpaging);
            if(data.question.Count==0){
                return Ok(new Response(){
                    status_code = 204,
                    message = "無資料，請新增題目"
                });
            }
            return Ok(new Response(){
                status_code = 200,
                message = "讀取成功",
                data = data
            });
        }
        // #endregion
    }
}