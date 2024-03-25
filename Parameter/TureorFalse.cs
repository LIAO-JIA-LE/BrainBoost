using System.ComponentModel;

namespace BrainBoost.Parameter
{
    public class TureorFalse
    {
        
        // 題目
        [DisplayName("題目")]
        public string question_content {get;set;}

        // 概念
        [DisplayName("標籤")]
        public string tag{get;set;}

        // 難度
        [DisplayName("難度")]
        public int question_level {get;set;}

        // 圖片
        [DisplayName("圖片")]
        public IFormFile? photo{get;set;}

        [DisplayName("答案")]
        public bool is_answer{get;set;}

        [DisplayName("解析")]
        public string parse{get;set;}
    }
}