using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuestAI.Parameter;

public class MemberLogin
{
    [DisplayName("帳號")]
    [Required(ErrorMessage = "請輸入帳號")]
    public string Member_Account { get; set; }
    [DisplayName("密碼")]
    [Required(ErrorMessage = "請輸入密碼")]
    public string Member_Password { get; set; }
}
