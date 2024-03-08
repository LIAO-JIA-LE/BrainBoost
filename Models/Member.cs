using System.ComponentModel;

namespace BrainBoost.Models;

public class Member
{
    [DisplayName("會員編號")]
    public int Member_Id { get; set; }
    [DisplayName("姓名")]
    public string Member_Name { get; set; }
    [DisplayName("用戶照片")]
    public string Member_Photo { get; set; }
    // [DisplayName("簡介")]
    // public string Member_Introduce { get; set; }
    [DisplayName("帳號")]
    public string Member_Account { get; set; }
    [DisplayName("密碼")]
    public string Member_Password { get; set; }
    [DisplayName("電子信箱")]
    public string Member_Email { get; set; }
    [DisplayName("驗證碼")]
    public string Member_AuthCode { get; set; }
}

