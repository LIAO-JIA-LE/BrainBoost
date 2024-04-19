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
                                        c.*
                                    FROM Class c
                                    JOIN Class_Member cm
                                    ON c.class_id = cm.class_id
                                    WHERE c.class_id = @class_id
                                ";
            string class_member_sql = $@"
                                            SELECT
                                                cm.*
                                            FROM Class c
                                            JOIN Class_Member cm
                                            ON c.class_id = cm.class_id
                                            WHERE c.class_id = @class_id
                                        ";
            ClassViewModel data = new();
            using var conn = new SqlConnection(cnstr);
            data.@class = conn.QueryFirst<Class>(class_sql,new{class_id});
            data.class_member = new List<Class_Member>(conn.Query<Class_Member>(class_member_sql,new{class_id}));
            return data;
        }

        //取得班級列表
        public List<Class> GetClassList(){
            string sql = $@"SELECT * FROM Class";
            using var conn = new SqlConnection(cnstr);
            return new List<Class>(conn.Query<Class>(sql));
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