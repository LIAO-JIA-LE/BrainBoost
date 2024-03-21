using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.ViewModels;
using Dapper;

namespace BrainBoost.Services
{
    public class RoleService
    {
        #region 宣告連線字串
        private readonly string? cnstr;
        public RoleService(IConfiguration configuration){
            cnstr = configuration.GetConnectionString("ConnectionStrings");
        }
        #endregion

        //獲取使用者角色名單
        public List<MemberRoleList> GetMemberRoleList(){
            List<MemberRoleList> data = new();
            string sql = $@"";
            using var conn = new SqlConnection(cnstr);
            return data;
        }
        //修改使用者權限(帳號)
        public void UpdateMemberRole(int member_id,int role){
            string sql = $@"UPDATE Member_Role SET role_id = {role} WHERE member_id = {member_id}";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql);
        }
    }
}