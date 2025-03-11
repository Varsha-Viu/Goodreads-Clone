using static API.Modals.Books;
using System.ComponentModel.DataAnnotations;

namespace API.Modals
{
    public class Genre
    {
        [Key]
        [MaxLength(450)]
        public string GenreId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
