using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost.ViewModels
{
    public class RaceQuestionAnswer
    {
        public int question_id{get;set;}
        public int question_level{get;set;}
        public string question_content{get;set;}
        public string question_picture{get;set;}
        public string option_content{get;set;}
        public string option_picture{get;set;}
        public bool is_delete{get;set;}

        public bool is_appear{get;set;}
    }
}