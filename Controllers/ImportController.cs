using Microsoft.AspNetCore.Mvc;
using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.Services;
using System.Data;


namespace BrainBoost.Controllers
{
    [Route("BrainBoost/[controller]")]
    [ApiController]
    public class ImportController : Controller
    {
        #region 呼叫函式
        private readonly QuestionsDBService QuestionService;
        private readonly MemberService MemberService;

        public ImportController(QuestionsDBService _questionService,MemberService _memberService)
        {
            QuestionService = _questionService;
            MemberService = _memberService;
        }
        #endregion

        #region 手動新增
        // 新增 是非題題目（手動）
        [HttpPost("[Action]")]
        public IActionResult TrueOrFalse([FromBody]TureorFalse question){
            
            // 將題目細節儲存至QuestionList物件
            QuestionList questionList = new();

            // 題目分類
            questionList.TagData.tag_name = question.tag;

            // 題目敘述
            questionList.QuestionData = new Question(){
                type_id = 1,
                question_level = question.question_level,
                question_content = question.question_content
            };
            
            // 題目答案
            questionList.AnswerData = new Answer(){
                question_answer = question.is_answer ? "是" : "否",
                question_parse = question.parse
            };

            try
            {
                questionList.QuestionData.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                // questionList.QuestionData.member_id = 1;
                QuestionService.InsertQuestion(questionList);
            }
            catch (Exception e)
            {
                return BadRequest($"發生錯誤:  {e}");
            }
            return Ok("新增成功");
        }

        // 新增 選擇題題目（手動）
        [HttpPost("[Action]")]
        public IActionResult MultipleChoice([FromBody]InsertQuestion question){
            
            // 將題目細節儲存至QuestionList物件
            QuestionList questionList = new();

            // 題目分類
            questionList.TagData.tag_name = question.tag;

            // 題目敘述
            questionList.QuestionData = new Question(){
                type_id = 2,
                question_level = question.question_level,
                question_content = question.question_content
            };

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
                // questionList.QuestionData.member_id = 1;
                QuestionService.InsertQuestion(questionList);
            }
            catch (Exception e)
            {
                return BadRequest($"發生錯誤:  {e}");
            }
            return Ok("新增成功");
        }

        // 新增 填充題題目（手動）
        [HttpPost("[Action]")]
        public IActionResult ShortAnswer([FromBody]InsertQuestion question){
            
            // 將題目細節儲存至QuestionList物件
            QuestionList questionList = new();

            // 題目分類
            questionList.TagData.tag_name = question.tag;

            // 題目敘述
            questionList.QuestionData = new Question(){
                type_id = 3,
                question_level = question.question_level,
                question_content = question.question_content
            };

            // 題目答案
            questionList.AnswerData = new Answer(){
                question_answer = question.answer,
                question_parse = question.parse
            };
            
            try
            {
                questionList.QuestionData.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                // questionList.QuestionData.member_id = 1;
                QuestionService.InsertQuestion(questionList);
            }
            catch (Exception e)
            {
                return BadRequest($"發生錯誤:  {e}");
            }
            return Ok("新增成功");
        }
        #endregion
        
        #region 檔案新增
        
        // 讀取 是非題Excel檔案
        [HttpPost("[Action]")]
        public IActionResult Excel_TrueorFalse(IFormFile file)
        {
            // 檔案處理
            DataTable dataTable = QuestionService.FileDataPrecess(file);
            // 將dataTable資料匯入資料庫
            foreach (DataRow dataRow in dataTable.Rows)
            {
                QuestionList question = new QuestionList();
                question.TagData.tag_name = dataRow["Tag"].ToString();

                question.QuestionData = new Question()
                {
                    type_id = 1,
                    question_level = Convert.ToInt32(dataRow["Level"]),
                    question_content = dataRow["Question"].ToString()
                };

                question.AnswerData = new Answer()
                {
                    question_answer = dataRow["Answer"].ToString(),
                    question_parse = dataRow["Parse"].ToString()
                };

                try
                {
                    question.QuestionData.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                    // question.QuestionData.member_id = 1;
                    QuestionService.InsertQuestion(question);
                }
                catch (Exception e)
                {
                    return BadRequest($"發生錯誤:  {e}");
                }
            }
            return Ok("匯入成功");    
        }
    
        // 讀取 選擇題Excel檔案
        [HttpPost("[Action]")]
        public IActionResult Excel_MultipleChoice(IFormFile file)
        {
            // 檔案處理
            DataTable dataTable = QuestionService.FileDataPrecess(file);
            // 將dataTable資料匯入資料庫
            foreach (DataRow dataRow in dataTable.Rows)
            {
                QuestionList question = new QuestionList();
                question.TagData.tag_name = dataRow["Tag"].ToString();

                question.QuestionData = new Question()
                {
                    type_id = 2,
                    question_level = Convert.ToInt32(dataRow["Level"]),
                    question_content = dataRow["Question"].ToString()
                };

                question.Options = new List<string>
                {
                    dataRow["OptionA"].ToString(),
                    dataRow["OptionB"].ToString(),
                    dataRow["OptionC"].ToString(),
                    dataRow["OptionD"].ToString()
                };

                question.AnswerData = new Answer()
                {
                    question_answer = dataRow["Answer"].ToString(),
                    question_parse = dataRow["Parse"].ToString()
                };

                try
                {
                    question.QuestionData.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                    QuestionService.InsertQuestion(question);
                }
                catch (Exception e)
                {
                    return BadRequest($"發生錯誤:  {e}");
                }
            }
            return Ok("匯入成功");    
        }
    
        // 讀取 填充題Excel檔案
        [HttpPost("[Action]")]
        public IActionResult Excel_ShortAnswer(IFormFile file)
        {
            // 檔案處理
            DataTable dataTable = QuestionService.FileDataPrecess(file);
            // 將dataTable資料匯入資料庫
            foreach (DataRow dataRow in dataTable.Rows)
            {
                QuestionList question = new QuestionList();
                question.TagData.tag_name = dataRow["Tag"].ToString();

                question.QuestionData = new Question()
                {
                    type_id = 3,
                    question_level = Convert.ToInt32(dataRow["Level"]),
                    question_content = dataRow["Question"].ToString()
                };
                question.AnswerData = new Answer()
                {
                    question_answer = dataRow["Answer"].ToString(),
                    question_parse = dataRow["Parse"].ToString()
                };

                try
                {
                    question.QuestionData.member_id = MemberService.GetDataByAccount(User.Identity.Name).Member_Id;
                    QuestionService.InsertQuestion(question);
                }
                catch (Exception e)
                {
                    return BadRequest($"發生錯誤:  {e}");
                }
            }
            return Ok("匯入成功");    
        }
        
        #endregion
    }
}
