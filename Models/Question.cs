using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BrainBoost.Models
{
    public class Question
    {
        // 題目編號
        public int question_id { get; set; }

        // 提型編號
        public int type_id { get; set; }

        // 科目編號
        public int subject_id { get; set; }

        // 會員編號
        public int member_id { get; set; }
        
        // 程度
        public int question_level{get;set;}

        // 題目敘述
        public string question_content { get; set; }

        // 題目照片
        public string? question_picture {  get; set; }

        // 新增時間
        public DateTime create_time {  get; set; }
    }
}
