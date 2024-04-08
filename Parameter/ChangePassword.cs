using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BrainBoost.Parameter
{
    public class ChangePassword
    {
        // 新密碼
        [DisplayName("密碼")]
        [Required(ErrorMessage = "請輸入密碼")]
        public string Password{get;set;}

        // 新密碼
        [DisplayName("新密碼")]
        [Required(ErrorMessage = "請輸入新密碼")]
        public string NewPassword{get;set;}
        // 確認新密碼
        [DisplayName("確認新密碼")]
        [Required(ErrorMessage = "請輸入確認新密碼")]
        [Compare("NewPassword", ErrorMessage = "兩個密碼不一致")]
        public string CheckNewPassword{get;set;}
    }
}