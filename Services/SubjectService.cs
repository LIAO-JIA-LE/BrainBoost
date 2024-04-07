using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.ViewModels;
using Dapper;

namespace BrainBoost.Services
{
    public class SubjectService(IConfiguration configuration,MemberService _memberService)
    {
        MemberService memberService = _memberService;
        private string? cnstr = configuration.GetConnectionString("ConnectionStrings");
        //查詢科目
        public List<Subject> GetAllSubject(int member_id){
            string sql = $@"SELECT * FROM ""Subject"" WHERE member_id = @member_id";
            using var conn = new SqlConnection(cnstr);
            return new List<Subject>(conn.Query<Subject>(sql,new {member_id = member_id}));
        }
        //新增科目
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
            conn.Execute(sql,new {member_id = insertData.teacher_id, insertData.subject_name});
        }

        //查詢科目詳細資料
        public SubjectViewModel GetSubject(int teacher_id,int subject_id){
            SubjectViewModel Data = new();
            string subject_sql = $@"
                                    SELECT 
                                        s.subject_id,
                                        s.member_id,
                                        s.subject_name
                                    FROM ""Subject"" s
                                    WHERE s.member_id = @teacher_id AND s.subject_id = @subject_id
                                ";
            string teacher_sql = $@"
                                    SELECT
                                        m.member_id,
                                        m.member_photo,
                                        m.member_account,
                                        m.member_name,
                                        m.member_email
                                    FROM Member m
                                    WHERE m.member_id = @teacher_id
                                ";
            string student_sql = $@"
                                    SELECT 
                                        m.member_id,
                                        m.member_photo,
                                        m.member_account,
                                        m.member_name,
                                        m.member_email
                                    FROM Member m
                                    JOIN(
                                        SELECT 
                                            sm.member_id 
                                        FROM ""Subject_Member"" sm
                                        JOIN ""Subject"" s
                                        ON s.subject_id = sm.subject_id
                                        WHERE s.member_id = @teacher_id AND s.subject_id = @subject_id
                                    )student
                                    ON m.member_id = student.member_id
                                ";
            using var conn = new SqlConnection(cnstr);
            Data.subject = conn.QueryFirstOrDefault<Subject>(subject_sql,new{teacher_id,subject_id});
            Data.teacher = conn.QueryFirstOrDefault<Member>(teacher_sql,new{teacher_id});
            Data.student_List = new List<Member>(conn.Query<Member>(student_sql,new{teacher_id,subject_id}));
            return Data;
        }
    
        //軟刪除科目
        public void DeleteSubject(int teacher_id,int subject_id){
            string sql = $@"UPDATE ""Subject"" s
                            SET s.is_delete = 1
                            WHERE s.subject_id = @subject_id AND teacher_id = @teacher_id";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new{ teacher_id, subject_id});
        }
        public void UpdateSubject(Subject subject){
            string sql = $@"UPDATE ""Subject"" s
                            SET s.subject_name = @subject_name
                            WHERE s.subject_id = @subjecct_id AND s.member_id = @member_id
                        ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,subject);
        }
    }
}