using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.ViewModels;
using Dapper;
using NPOI.SS.Formula.Functions;

namespace BrainBoost.Services
{
    public class ClassService(IConfiguration _configuration)
    {
        private readonly string? cnstr = _configuration.GetConnectionString("ConnectionStrings");
        //新增班級
        public int InsertClass(InsertClass Data){
            string sql = $@"
                            DECLARE @ClassID INT
                            INSERT INTO ""Class""(class_name,member_id)
                            VALUES(@class_name,@member_id)
                            SET @ClassID = SCOPE_IDENTITY()
                        ";
            foreach(int student_id in Data.List_student_id){
                sql += @$"INSERT INTO ""Class_Member""(class_id,member_id)
                          VALUES(@ClassID," + student_id + ");";
            }
            sql += @"SELECT @ClassID";
            using var conn = new SqlConnection(cnstr);
            int class_id = conn.QueryFirstOrDefault<int>(sql,new{Data.class_name, member_id = Data.teacher_id});
            return class_id;
        }
        //查詢班級資訊
        public ClassViewModel GetClassViewModel(int class_id){
            string class_sql = $@"
                                    SELECT 
                                        c.class_id,
                                        c.class_name,
                                        c.member_id,
                                        COUNT(c.class_id) ""count""
                                    FROM Class c
                                    JOIN Class_Member cm
                                    ON c.class_id = cm.class_id
                                    WHERE c.is_delete = 0 AND c.class_id = @class_id
                                    GROUP BY c.class_id,c.class_name,c.member_id
                                ";
            string class_member_sql = $@"
                                            SELECT
                                                cm.*
                                            FROM Class c
                                            JOIN Class_Member cm
                                            ON c.class_id = cm.class_id
                                            WHERE c.class_id = @class_id
                                        ";
            string member_sql = $@"
                                    SELECT
                                        m.member_id,
                                        m.member_name,
                                        m.member_photo,
                                        m.member_account,
                                        m.member_email
                                    FROM Member m
                                    JOIN Class_Member cm
                                    ON m.member_id = cm.member_id
                                    WHERE cm.class_id =  @class_id
                                ";
            ClassViewModel data = new();
            using var conn = new SqlConnection(cnstr);
            data.@class = conn.QueryFirst<Class>(class_sql,new{class_id});
            //班級學生
            var class_Members = conn.Query<Class_Member>(class_member_sql,new{class_id}).AsList();
            data.@class.count = class_Members.Count;
            //學生詳細資料
            var members = conn.Query<Member>(member_sql,new{class_id}).AsList();
            //合併
            var students = class_Members.Zip(members,(classMember, member) => new Student(member, classMember)).ToList();
            data.students = students;
            return data;
        }

        //取得班級列表
        public List<Class> GetClassList(){
            string sql = $@"SELECT 
                                c.class_id,
                                c.class_name,
                                c.member_id,
                                COUNT(c.class_id) ""count""
                            FROM Class c
                            JOIN Class_Member cm
                            ON c.class_id = cm.class_id
                            WHERE c.is_delete = 0
                            GROUP BY c.class_id,c.class_name,c.member_id";
            using var conn = new SqlConnection(cnstr);
            List<Class> data = new(conn.Query<Class>(sql));
            return data;
        }
        //刪除班級
        public void DeleteClass(DeleteClass deleteData){
            string sql = $@"UPDATE Class SET is_delete = 1
                            WHERE class_id = @class_id
                            ";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,new{deleteData});
        }
        //更新班級資訊
        public void UpdateClass(UpdateClass updateData){
            string sql = $@"UPDATE Class SET class_name = @class_name, member_id = @teacher_id
                            WHERE class_id = @class_id";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,updateData);
        }
        //班級新增學生
        public void InsertStudent(ClassStudent insertData){
            string sql = $@"INSERT INTO Class_Member(class_id,member_id)
                            VALUES(@class_id,@member_id)";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,insertData);
        }
        //班級刪除學生
        public void DeleteStudent(ClassStudent deleteData){
            string sql = $@"DELETE Class_Member WHERE class_id = @class_id AND member_id = @member_id";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql,deleteData);
        }
    }
}