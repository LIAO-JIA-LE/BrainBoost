using BrainBoost.Parameter;
using BrainBoost.Services;
using Microsoft.AspNetCore.Mvc;
using BrainBoost.Models;
using Microsoft.IdentityModel.Tokens;


namespace BrainBoost.Controllers
{
    [Route("[controller]")]
    public class QuestionController : Controller
    {
        private readonly QuestionsDBService QuestionService;
        readonly MemberService MemberService;

        public QuestionController(QuestionsDBService _questionService,MemberService _memberService)
        {
            QuestionService = _questionService;
            MemberService = _memberService;
        }

        // 新增 選擇題題目（手動）
        [HttpPost("[Action]")]
        public IActionResult Insert_Mcq(InsertQuestion question){
            
            // 將題目細節儲存至QuestionList物件
            QuestionList questionList = new();
            questionList.QuestionData.type_id = 2;
            // 題目敘述
            questionList.QuestionData.question_content = question.question_content;
            // 題目選項
            questionList.Options = new List<string>(){
                question.optionA.ToString(),
                question.optionB.ToString(),
                question.optionC.ToString(),
                question.optionD.ToString(),
            };
            // 題目答案
            questionList.AnswerData = new Answer(){
                question_answer = question.answer,
                question_parse = question.parse
            };

            try
            {
                questionList.QuestionData.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                QuestionService.InsertQuestion(questionList);
            }
            catch (Exception e)
            {
                return BadRequest($"發生錯誤:  {e}");
            }
            return Ok("");
        }
<<<<<<< HEAD
    
        // 顯示 選擇題題目
        // [HttpPost("[Action]")]
        // public
=======

        [HttpPost("[Action]")]
        public IActionResult Get_Question(int page = 1){
            
            return Ok();
        }
>>>>>>> 9fd1f1e39c62f7a74ad358760084067b6ebcc4e3
    }
}