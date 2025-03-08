using System.ComponentModel.DataAnnotations;

namespace API.Modals
{
    public class AuthorSocialLinks
    {
        [Required]
        [Key]
        public int SocialId { get; set; }
        [Required]
        public Guid AuthorId { get; set; }
        [Required]
        public string Platform { get; set; }
        [Required]
        public string Url { get; set; }
    }
}
