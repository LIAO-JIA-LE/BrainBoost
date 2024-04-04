using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost.Models;
using BrainBoost.Parameter;
using Dapper;

namespace BrainBoost.Services
{
    public class SubjectService(IConfiguration configuration)
    {
        private string? cnstr = configuration.GetConnectionString("ConnectionStrings");
        //查詢科目
        public List<Subject> GetAllSubject(int member_id){
            string sql = $@"SELECT * FROM ""Subject"" WHERE member_id = @member_id";
            using var conn = new SqlConnection(cnstr);
            return new List<Subject>(conn.Query<Subject>(sql,new {member_id = member_id}));
        }
        public void InsertSubject(InsertSubject insertData){
            string sql = $@"
                DECLARE @SubjectID INT
                INSERT INTO ""Subject""(member_id,subject_name)
                VALUES(@member_id,@subject_name)
                
                SET @SubjectID = SCOPE_IDENTITY();
            ";
            foreach(int student_id in insertData.List_student_id){
                sql += $@"
                    INSERT INTO ""Subject_Member""(subject_id,member_id)
                    VALUES(@SubjectID,{student_id})
                ";
            }
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new {member_id = insertData.teacher_id, subject_name = insertData.subject_name});
        }
    }
}