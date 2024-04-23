using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost.ViewModels
{
    public class StudentResponse
    {
        // 題目編號
        public int question_id{get;set;}

        // 會員編號
        public int member_id{get;set;}

        // 限時時間
        public int time_limit{get;set;}

        // 答題時間
        public float time_response{get;set;}

        // 學生答案
        public string race_answer{get;set;}
    }
}