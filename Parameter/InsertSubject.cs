using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost.Parameter
{
    public class InsertSubject
    {
        //科目名稱
        public string subject_name{get;set;}
        //老師帳號
        public int teacher_id{get;set;}
        //學生編號列表
        public List<int> List_student_id{get;set;}
    }
}