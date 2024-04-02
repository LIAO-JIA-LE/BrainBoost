using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BrainBoost.Models;
using Dapper;

namespace BrainBoost.Services
{
    public class SubjectService(IConfiguration configuration)
    {
        private string? cnstr = configuration.GetConnectionString("ConnectionStrings");
        //查詢科目
        public List<Subject> GetAllSubject(int member_id){
            string sql = $@"SELECT * FROM ""Subject"" WHERE member_id = @member_id";
            using var conn = new SqlConnection(sql);
            return new List<Subject>(conn.Query<Subject>(sql));
        }
    }
}