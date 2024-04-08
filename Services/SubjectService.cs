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
        // 連線字串
        private string? cnstr = configuration.GetConnectionString("ConnectionStrings");

        #region 科目
        // 查詢科目
        public List<Subject> GetAllSubject(int member_id){
            string sql = $@"SELECT * FROM ""Subject"" WHERE member_id = @member_id";
            using var conn = new SqlConnection(cnstr);
            return new List<Subject>(conn.Query<Subject>(sql,new {member_id = member_id}));
        }

        // 新增科目
        public void InsertSubject(InsertSubject insertData){
            string sql = $@"DECLARE @SubjectID INT
                            INSERT INTO ""Subject""(member_id,subject_name)
                            VALUES(@member_id,@subject_name)
                            SET @SubjectID = SCOPE_IDENTITY();";
            foreach(int student_id in insertData.List_student_id){
                sql += $@"INSERT INTO ""Subject_Member""(subject_id,member_id)
                            VALUES(@SubjectID,{student_id})";
            }
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new {member_id = insertData.teacher_id, subject_name = insertData.subject_name});
        }

        // 修改科目
        public void UpdateSubjectName(InsertSubject insertData){
            string sql = $@"UPDATE Subject SET subject_name = '@subject_name' WHERE member_id = @member_id";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new {member_id = insertData.teacher_id, subject_name = insertData.subject_name});
        }

        // 新增學生
        public void InsertStudent(int subject_id, int student_id){
            string sql = $@"INSERT INTO ""Subject_Member""(subject_id,member_id)
                            VALUES(@subject_id, @student_id)";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new {subject_id, student_id});
        }

        // 刪除學生
        public void DeleteStudent(int subject_id, int student_id){
            string sql = $@"DELETE FROM ""Subject_Member"" WHERE subject_id = @subject_id AND member_id = @student_id";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new {subject_id, student_id});
        }
        #endregion
    }
}