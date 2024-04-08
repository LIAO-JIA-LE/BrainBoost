using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost.Models;

namespace BrainBoost.ViewModels
{
    public class SubjectViewModel
    {
        public Subject subject {get;set;} = new();
        public Member teacher {get;set;} = new();
        public List<Member> student_List {get;set;} = [];
    }
}