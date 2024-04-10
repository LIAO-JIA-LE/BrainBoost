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
                            DELARE @ClassID INT
                            INSERT INTO ""Class""(class_name,member_id)
                            VALUES(@class_name,@member_id)
                            SET @ClassID = SCOPE_IDENTITY()

                            INSERT INTO ""Class_Member""
                        ";
            foreach(int student_id in Data.List_student_id){
                // sql += "" + i.
            }
        }
    }
}