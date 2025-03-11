using System.ComponentModel.DataAnnotations;

namespace API.Modals
{
    public class Publishers
    {
        [Key]
        [MaxLength(450)]
        public string PublisherId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Url]
        [MaxLength(500)]
        public string? WebsiteUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
