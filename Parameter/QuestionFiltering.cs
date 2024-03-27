using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost.Services;

namespace BrainBoost.ViewModels
{
    public class QuestionFiltering
    {
        //會員編號
        public int member_id{get;set;}
        
        // 題型
        public int? type_id{get;set;}
        
        // 概念
        public int? tag_id{get;set;}

        // 難易度
        public int? question_level{get;set;}

        // 科目
        public int? subject_id{get;set;}

        // 搜尋
        public string? search{get;set;}

        // 分頁
        // public Forpaging paging{get;set;}
    }
}