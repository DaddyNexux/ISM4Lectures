namespace FuckingLectures.Models.DTOs.Auth
{
    public class LoginResDTO
    {
        public string Token { get; set; }
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }

        public List<string>? Permissions { get; set; }
    }
}
