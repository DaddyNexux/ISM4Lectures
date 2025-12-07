namespace FuckingLectures.Models.DTOs.Auth
{
    public class ForgetPasswordDTO
    {
        public string PhoneNumber { get; set; } 
       
    }

    public class ForgetPasswordVerifyDTO
    {
        public string PhoneNumber { get; set; }
        public string OTPCode { get; set; }
        public string NewPassword { get; set; }
    }
}
