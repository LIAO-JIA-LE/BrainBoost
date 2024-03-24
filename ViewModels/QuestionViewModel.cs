using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost.Models;
using BrainBoost.Services;

namespace BrainBoost.ViewModels
{
    public class QuestionViewModel
    {
        public Forpaging forpaging {get;set;}
        public List<Question> question  {get;set;}
    }
}