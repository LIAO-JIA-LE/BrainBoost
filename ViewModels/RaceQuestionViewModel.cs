using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost.Models;

namespace BrainBoost.ViewModels
{
    public class RaceQuestionViewModel
    {
        public Question question{get;set;}
        public List<Option> options{get;set;}
    }
}