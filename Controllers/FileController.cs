using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.Services;
using System.Globalization;
using System;
using System.Data;
using System.IO;
using Microsoft.AspNetCore.Http;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;


namespace BrainBoost.Controllers
{
    [Route("BrainBoost/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        #region 呼叫函式
        private readonly QuestionsDBService QuestionService;
        private readonly MemberService MemberService;

        public FileController(QuestionsDBService _questionService,MemberService _memberService)
        {
            QuestionService = _questionService;
            MemberService = _memberService;
        }
        #endregion

        #region 檔案匯入
        
        // 讀取 是非題Excel檔案
        [HttpPost("[Action]")]
        public IActionResult UploadExcel_Tfq(IFormFile file)
        {
            // 檔案處理
            DataTable dataTable = QuestionService.FileDataPrecess(file);
            // 將dataTable資料匯入資料庫
            foreach (DataRow dataRow in dataTable.Rows)
            {
                QuestionList question = new QuestionList();

                question.QuestionData = new Question()
                {
                    type_id = 1,
                    question_content = dataRow["Question"].ToString()
                };

                question.AnswerData = new Answer()
                {
                    question_answer = dataRow["Answer"].ToString(),
                    question_parse = dataRow["Parse"].ToString()
                };

                try
                {
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
        public IActionResult UploadExcel_Mcq(IFormFile file)
        {
            // 檔案處理
            DataTable dataTable = QuestionService.FileDataPrecess(file);
            // 將dataTable資料匯入資料庫
            foreach (DataRow dataRow in dataTable.Rows)
            {
                QuestionList question = new QuestionList();

                question.QuestionData = new Question()
                {
                    type_id = 2,
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
        public IActionResult UploadExcel_Fq(IFormFile file)
        {
            // 檔案處理
            DataTable dataTable = QuestionService.FileDataPrecess(file);
            // 將dataTable資料匯入資料庫
            foreach (DataRow dataRow in dataTable.Rows)
            {
                QuestionList question = new QuestionList();

                question.QuestionData = new Question()
                {
                    type_id = 3,
                    question_content = dataRow["Question"].ToString()
                };

                question.AnswerData = new Answer()
                {
                    question_answer = dataRow["Answer"].ToString(),
                    question_parse = dataRow["Parse"].ToString()
                };

                try
                {
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
