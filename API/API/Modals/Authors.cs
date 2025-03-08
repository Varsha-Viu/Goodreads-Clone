using System.ComponentModel.DataAnnotations;

namespace API.Modals
{
    public class Authors
    {
        [Required]
        [Key]
        public Guid AuthorId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public string? PenName { get; set; }
        public string? Bio { get; set; }
    }
}
