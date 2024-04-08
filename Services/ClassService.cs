using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost.ViewModels;

namespace BrainBoost.Services
{
    public class ClassService
    {
        
        //新增班級
        public void InsertClass(ClassViewModel Data){
            string sql = $@"
                            INSERT INTO ""Class""(class_name,member_id)
                            VALUES(@class_name,@member_id)
                            DELARE
                        ";
        }
    }
}