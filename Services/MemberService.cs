using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using BrainBoost.Models;
using BrainBoost.Parameter;
using BrainBoost.ViewModels;
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

    // 用account獲得資料
    public Member GetDataByAccount(string account){
        string sql = $@"SELECT * FROM Member WHERE member_account = '{account}' ";
        using (var conn = new SqlConnection(cnstr))
        return conn.QueryFirstOrDefault<Member>(sql);
    }

    // 用mail獲得資料
    public Member GetDataByEmail(string mail){
        string sql = $@"SELECT * FROM Member WHERE Member_Email = '{mail}' ";
        using (var conn = new SqlConnection(cnstr))
        return conn.QueryFirstOrDefault<Member>(sql);
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

    //註冊
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

    //信箱驗證
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

    //(後台管理者)
    //取得所有使用者
    public List<Member> GetAllMemberList(string Search,Forpaging forpaging){
        List<Member> Data = new();
        //判斷是否有增加搜尋值
        if(string.IsNullOrEmpty(Search)){
            SetMaxPage(forpaging);
            Data = GetMemberList(forpaging);
        }
        else{
            SetMaxPage(Search,forpaging);
            Data = GetMemberList(Search,forpaging);
        }
        return Data;
    }
    //無搜尋值查詢的使用者列表
    public List<Member> GetMemberList(Forpaging forpaging){
        string sql = $@"SELECT * FROM (
                            SELECT ROW_NUMBER() OVER(ORDER BY m.member_id DESC) r_num,m.*,mr.role_id FROM Member m 
                            JOIN Member_Role mr
                            ON m.member_id = mr.member_id
                        )a
                        WHERE a.r_num BETWEEN {(forpaging.NowPage - 1) * forpaging.Item + 1} AND {forpaging.NowPage * forpaging.Item }";
        using var conn = new SqlConnection(cnstr);
        
    }
    //有搜尋值查詢的使用者列表
    public List<Member> GetMemberList(string Search,Forpaging forpaging){

    }
    //無搜尋值計算所有使用者並設定頁數
    public void SetMaxPage(Forpaging forpaging){
        string sql = $@"SELECT COUNT(*) FROM Member";
        using var conn = new SqlConnection(cnstr);
        int row = conn.QueryFirst(sql);
        forpaging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / 10));
        forpaging.SetRightPage();
    }
    //有搜尋值計算所有使用者並設定頁數
    public void SetMaxPage(string Search,Forpaging forpaging){
        string sql = $@"SELECT COUNT(*) FROM Member WHERE member_account LIKE '%{Search}%' OR member_name LIKE '%{Search}%'";
        using var conn = new SqlConnection(cnstr);
        int row = conn.QueryFirst(sql);
        forpaging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(row) / 10));
        forpaging.SetRightPage();
    }
}