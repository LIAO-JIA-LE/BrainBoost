using System.ComponentModel;

namespace BrainBoost.Models;

public class Member
{
    // 會員編號
    [DisplayName("會員編號")]
    public int Member_Id { get; set; }
    
    // 姓名
    [DisplayName("姓名")]
    public string Member_Name { get; set; }
    
    // 會員照片
    [DisplayName("會員照片")]
    public string Member_Photo { get; set; }
    
    // 會員簡介
    // [DisplayName("簡介")]
    // public string Member_Introduce { get; set; }

    // 會員帳號
    [DisplayName("帳號")]
    public string Member_Account { get; set; }
    

    // 會員密碼
    [DisplayName("密碼")]
    public string Member_Password { get; set; }
    

    // 會員信箱
    [DisplayName("電子信箱")]
    public string Member_Email { get; set; }
    

    // 會員驗證碼
    [DisplayName("驗證碼")]
    public string Member_AuthCode { get; set; }
    

    // 會員權限
    [DisplayName("權限")]
    public int Member_Role { get; set; }
}

