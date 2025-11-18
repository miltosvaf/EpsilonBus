using System.ComponentModel.DataAnnotations;

namespace EpsilonBus.Api.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Role { get; set; } // Added role property
    }
}
 