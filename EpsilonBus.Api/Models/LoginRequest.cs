namespace EpsilonBus.Api.Models
{
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse  
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // Added role property
    }
}
