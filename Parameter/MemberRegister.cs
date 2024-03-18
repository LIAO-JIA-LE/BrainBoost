using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


public class MemberRegister
{
    // 會員名稱
    [DisplayName("用戶名稱")]
    [Required(ErrorMessage = "請輸入用戶名稱")]
    public required string Member_Name { get; set; }

    // 會員帳號
    [DisplayName("帳號")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "帳號長度需介於6-20字元")]
    [DataType(DataType.Password,ErrorMessage ="請輸入英文及數字")]
    [Required(ErrorMessage ="請輸入帳號")]
    public required string Member_Account { get; set; }
    
    // 會員密碼
    [DisplayName("密碼")]
    [DataType(DataType.Password, ErrorMessage = "請輸入英文及數字")]
    [Required(ErrorMessage = "請輸入密碼")]
    public required string Member_Password { get; set; }
    
    // 確認密碼
    [DisplayName("確認密碼")]
    [Required(ErrorMessage = "請輸入確認密碼")]
    [Compare("Member_Password",ErrorMessage ="兩次密碼不一致")]
    public required string Member_PasswordCheck { get; set; }
    
    // 電子信箱
    [DisplayName("電子信箱")]
    [Required(ErrorMessage = "請輸入電子信箱")]
    [EmailAddress(ErrorMessage ="電子信箱格式不正確")]
    public required string Member_Email { get; set; }
}

