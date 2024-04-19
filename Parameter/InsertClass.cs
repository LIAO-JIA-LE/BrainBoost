using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost.Parameter
{
    public class InsertClass
    {
        public string class_name {get;set;}
        public int teacher_id;
        public List<int> List_student_id {get;set;}
    }
}