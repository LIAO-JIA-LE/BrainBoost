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
        #region 呼叫函示
        private readonly QuestionsDBService QuestionService;
        readonly MemberService MemberService;

        public QuestionController(QuestionsDBService _questionService,MemberService _memberService)
        {
            QuestionService = _questionService;
            MemberService = _memberService;
        }
        #endregion

        #region 手動新增
        // 新增 是非題題目（手動）
        [HttpPost("[Action]")]
        public IActionResult Insert_Tfq(InsertQuestion question){
            
            // 將題目細節儲存至QuestionList物件
            QuestionList questionList = new();
            questionList.QuestionData.type_id = 1;
            // 題目敘述
            questionList.QuestionData.question_content = question.question_content;
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

        // 新增 填充題題目（手動）
        [HttpPost("[Action]")]
        public IActionResult Insert_Fq(InsertQuestion question){
            
            // 將題目細節儲存至QuestionList物件
            QuestionList questionList = new();
            questionList.QuestionData.type_id = 3;
            // 題目敘述
            questionList.QuestionData.question_content = question.question_content;
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
        #endregion
        
        #region 顯示問題
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
        #endregion
    }
}