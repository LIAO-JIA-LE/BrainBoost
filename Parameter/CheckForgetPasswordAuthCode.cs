using System;

namespace BrainBoost.Parameter{
    public class CheckForgetPasswordAuthCode{
        // 信箱
        public string Email{get;set;}
        // 驗證碼
        public string AuthCode{get;set;}
    }
}