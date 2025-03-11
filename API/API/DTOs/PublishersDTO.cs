using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class AddPublishersDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? WebsiteUrl { get; set; }
    }
}

