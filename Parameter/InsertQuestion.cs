using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BrainBoost.Models;

namespace BrainBoost.Parameter
{
    public class InsertQuestion
    {
        // 題目
        [DisplayName("題目")]
        public string question_content {get;set;}

        // 圖片
        [DisplayName("圖片")]
        public IFormFile? photo{get;set;}

        // 選項A
        [DisplayName("選項A")]
        public string optionA {get;set;}

        // 選項B
        [DisplayName("選項B")]
        public string optionB {get;set;}

        // 選項C
        [DisplayName("選項C")]
        public string optionC {get;set;}

        // 選項D
        [DisplayName("選項D")]
        public string optionD {get;set;}

        // 答案
        //前端用JS回傳已輸入的選項當作正確答案回傳值
        [DisplayName("答案")]
        public string answer{get;set;}

        // 解析
        [DisplayName("解析")]
        public string? parse{get;set;}
    }
}