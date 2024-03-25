using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost.Parameter
{
    public class InsertRoom
    {
        public string race_name{get;set;}
        public bool race_function{get;set;}
        public bool race_public{get;set;}
        public int time_limit{get;set;}
        public List<int> question_id{get;set;}
    }
}