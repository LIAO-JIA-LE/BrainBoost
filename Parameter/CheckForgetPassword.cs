using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BrainBoost.Parameter{
    public class CheckForgetPassword{
        // 新密碼
        [DisplayName("新密碼")]
        [Required(ErrorMessage = "請輸入新密碼")]
        public string NewPassword{get;set;}
        // 確認新密碼
        [DisplayName("確認新密碼")]
        [Required(ErrorMessage = "請輸入確認新密碼")]
        [Compare("NewPassword", ErrorMessage = "兩個密碼不一致")]
        public string CheckNewPassword{get;set;}
        // Email
        public string Email{get;set;}
    }
}