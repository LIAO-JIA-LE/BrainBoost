using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost.Models
{
    //班級名單
    public class Class_Member
    {
        //班級學生資料表編號
        public int class_member_id {get;set;}
        //班級編號
        public int class_id {get;set;}
        //學生編號
        public int member_id = new();
    }
}