using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using BrainBoost.Models;
using BrainBoost.Parameter;
using Dapper;

namespace BrainBoost.Services;

public class MemberService
{
    #region 宣告連線字串
    private readonly string? cnstr;
    public MemberService(IConfiguration configuration){
        cnstr = configuration.GetConnectionString("ConnectionStrings");
    }
    #endregion

    #region 登入
    // 獲得權限
    public int GetRole(string member_Account)
    {
        string sql = $@"SELECT m_r.role_id FROM Member m INNER JOIN Member_Role m_r ON m.member_id = m_r.member_id WHERE m.member_account = '{member_Account}'";
        using var conn = new SqlConnection(cnstr);
        int role_id = conn.QueryFirstOrDefault<int>(sql);
        return role_id;
    }

    // 登入確認
    public string LoginCheck(string member_Account, string member_Password)
    {
        Member Data = GetDataByAccount(member_Account);
        if (Data != null)
        {
            if (String.IsNullOrWhiteSpace(Data.Member_AuthCode))
            {
                if (PasswordCheck(Data.Member_Password, member_Password))
                    return "";
                else
                    return "密碼錯誤";
            }
            else
                return "此帳號尚未經過Email驗證";
        }
        else
            return "無此會員資料";
    }
    #endregion
    
    #region 註冊
    // 密碼確認
    public bool PasswordCheck(string Data, string Password)
    {
        return Data.Equals(HashPassword(Password));
    }

    // 確認註冊
    public string RegisterCheck(string Account,string Email){
        if(GetDataByAccount(Account)!=null)
            return "帳號已被註冊";
        else if(GetDataByEmail(Email)!=null)
            return "電子郵件已被註冊";
        return "";
    }

    // 雜湊密碼
    public string HashPassword(string Password)
    {
        using (SHA512 sha512 = SHA512.Create())
        {
            string salt = "foiw03pltmvle6";
            string saltandpas = string.Concat(salt, Password);
            byte[] data = Encoding.UTF8.GetBytes(saltandpas);
            byte[] hash = sha512.ComputeHash(data);
            string result = Convert.ToBase64String(hash);
            return result;
        }
    }

    // 註冊
    public void Register(Member member)
    {
        string sql = @$"INSERT INTO Member(member_name,member_account,member_password,member_email,member_authcode)
                                    VALUES('{member.Member_Name}','{member.Member_Account}','{member.Member_Password}','{member.Member_Email}','{member.Member_AuthCode}')
                        /*設定暫時的變數*/
                        DECLARE @member_id int = (SELECT m.member_id FROM Member m WHERE m.member_account = '{member.Member_Account}');
                        INSERT INTO Member_Role(member_id,role_id)
                                    VALUES(@member_id,0)";
        using var conn = new SqlConnection(cnstr);
        conn.Execute(sql);
    }

    // 郵件驗證
    public bool MailValidate(string Account, string AuthCode)
    {
        Member Data = GetDataByAccount(Account);
        //判斷有無會員資料並比對驗證碼是否正確
        if (Data != null && Data.Member_AuthCode == AuthCode)
        {
            string sql = $@"UPDATE Member SET member_authcode = '{string.Empty}' WHERE member_account = '{Account}'";
            using var conn = new SqlConnection(cnstr);
            conn.Execute(sql);
            return Data.Member_AuthCode == AuthCode;
        }
        else return false;
    }
    #endregion
    
    #region 忘記密碼
    // 更新驗證碼
    public void ChangeAuthCode(string NewAuthCode,string Email){
        string sql = $@" UPDATE Member SET member_authcode = '{NewAuthCode}' WHERE member_email = '{Email}';";
        using (var conn = new SqlConnection(cnstr))
        conn.Execute(sql);
    }

    // 修改權限‵‵‵‵‵‵‵‵‵‵‵‵‵‵‵
    public void ChangeMemberRole(int id, int Role){
        string sql = $@" UPDATE Member SET Member_Role = {Role} WHERE Member_Id = {id} ";
        using (var conn = new SqlConnection(cnstr))
        conn.Execute(sql);
    }

    // 清除驗證碼
    public void ClearAuthCode(string Email){
        string sql = $@" UPDATE Member SET member_authcode = '{String.Empty}' WHERE member_email = '{Email}';";
        using (var conn = new SqlConnection(cnstr))
        conn.Execute(sql);
    }

    // 更改密碼ByForget
    public void ChangePasswordByForget(CheckForgetPassword Data){
        Member member = GetDataByEmail(Data.Email);
        member.Member_Password = HashPassword(Data.NewPassword);
        string sql = $@" UPDATE Member SET member_password = '{member.Member_Password}' WHERE member_email = '{Data.Email}';";
        using (var conn = new SqlConnection(cnstr))
        conn.Execute(sql);
    }
    #endregion

    #region 獲得資料
    // 用account獲得資料
    public Member GetDataByAccount(string account){
        string sql = $@"SELECT * FROM Member WHERE member_account = '{account}' ";
        using (var conn = new SqlConnection(cnstr))
        return conn.QueryFirstOrDefault<Member>(sql);
    }

    // 用mail獲得資料
    public Member GetDataByEmail(string mail){
        string sql = $@"SELECT * FROM Member WHERE member_email = '{mail}' ";
        using (var conn = new SqlConnection(cnstr))
        return conn.QueryFirstOrDefault<Member>(sql);
    }
    #endregion
}