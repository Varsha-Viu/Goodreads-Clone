using System.ComponentModel.DataAnnotations;
using API.Constants;

namespace API.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public UserRoles Role { get; set; }
    }

    public class LoginDTO
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
