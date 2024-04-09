using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost.ViewModels
{
    public class StudentReseponse
    {
        // 搶答室編號
        public int raceroom_id{get;set;}

        // 題目編號
        public int question_id{get;set;}

        // 會員編號
        public int member_id{get;set;}

        // 學生答案
        public string race_answer{get;set;}
    }
}