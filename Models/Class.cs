using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost.Models
{
    //班級
    public class Class
    {
        //班級編號
        public int class_id {get;set;}
        //班級名稱
        public string class_name {get;set;}
        //導師編號
        public int member_id {get;set;}
        //班級人數
        public int count {get;set;}
    }
}