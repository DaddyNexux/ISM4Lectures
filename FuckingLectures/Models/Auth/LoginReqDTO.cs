namespace FuckingLectures.Models.DTOs.Auth
{
    public class LoginReqDTO
    {
        public string? Username { get; set; }
        public string? phoneNumber { get; set; }
        public required string Password { get; set; }
    }
}
