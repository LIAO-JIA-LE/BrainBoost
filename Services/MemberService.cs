using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using BrainBoost.Models;
using BrainBoost.Parameter;
using Dapper;

namespace BrainBoost.Services;

public class MemberService
{
    private readonly string? cnstr;
    public MemberService(IConfiguration configuration){
        cnstr = configuration.GetConnectionString("ConnectionStrings");
    }
    public int GetRole(string member_Account)
    {
        return GetDataByAccount(member_Account).Member_Role;
    }

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
    public Member GetDataByAccount(string account){
        string sql = $@"SELECT * FROM Member WHERE member_account = '{account}' ";
        using (var conn = new SqlConnection(cnstr))
        return conn.QueryFirstOrDefault<Member>(sql);
    }
    public Member GetDataByEmail(string mail){
        string sql = $@"SELECT * FROM Member WHERE Member_Email = '{mail}' ";
        using (var conn = new SqlConnection(cnstr))
        return conn.QueryFirstOrDefault<Member>(sql);
    }
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

    public void Register(Member member)
    {
        string sql = @$"INSERT INTO Member(member_name,member_account,member_password,member_email,member_authcode,member_role)
                                    VALUES('{member.Member_Name}','{member.Member_Account}','{member.Member_Password}','{member.Member_Email}','{member.Member_AuthCode}','{member.Member_Role}')";
        using var conn = new SqlConnection(cnstr);
        conn.Execute(sql);
    }

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
}