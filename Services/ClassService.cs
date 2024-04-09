using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost.Parameter;
using BrainBoost.ViewModels;

namespace BrainBoost.Services
{
    public class ClassService
    {
        
        //新增班級
        public void InsertClass(InsertClass Data){
            string sql = $@"
                            INSERT INTO ""Class""(class_name,member_id)
                            VALUES(@class_name,@member_id)
                            DELARE 
                        ";
        }
    }
}