using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BrainBoost.Parameter;

public class MemberLogin
{
    // 帳號
    [DisplayName("帳號")]
    [Required(ErrorMessage = "請輸入帳號")]
    public string Member_Account { get; set; }

    // 密碼
    [DisplayName("密碼")]
    [Required(ErrorMessage = "請輸入密碼")]
    public string Member_Password { get; set; }
}
