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
        private readonly QuestionsDBService questionsDBService;

        public QuestionController(QuestionsDBService _questionsDBService)
        {
            questionsDBService = _questionsDBService;
        }

        //手動輸入選擇題題目
        [HttpPost("[Action]")]
        public IActionResult Insert_Mcq(InsertQuestion question){
            
            //將題目細節儲存至QuestionList物件
            QuestionList questionList = new();
            questionList.QuestionData.type_id = 2;
            //題目敘述
            questionList.QuestionData.question_content = question.question_content;
            //題目選項
            questionList.Options = new List<string>(){
                question.optionA.ToString(),
                question.optionB.ToString(),
                question.optionC.ToString(),
                question.optionD.ToString(),
            };
            //題目答案
            questionList.AnswerData = new Answer(){
                question_answer = question.answer,
                question_parse = string.IsNullOrEmpty(question.answer) ? question.parse : ""
            };

            try
            {
                questionsDBService.InsertQuestion(questionList);
            }
            catch (Exception e)
            {
                return BadRequest($"發生錯誤:  {e}");
            }
            return Ok("");
        }
    }
}