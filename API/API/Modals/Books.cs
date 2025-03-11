using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Modals
{
    public class Books
    {
        [MaxLength(250)]
        [Key]
        public string BookId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [MaxLength(255)]
        public string? Subtitle { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        [MaxLength(20)]
        public string ISBN { get; set; }

        [Required]
        public string GenreId { get; set; }  // Foreign Key

        [Required]
        public Guid AuthorId { get; set; }  // Foreign Key

        public string? PublisherId { get; set; }  // Optional Foreign Key

        [Required]
        public int PublicationYear { get; set; }

        [Required]
        [MaxLength(100)]
        public string Language { get; set; }

        [Required]
        public int PageCount { get; set; }

        public string CoverImageUrl { get; set; }

        public decimal? AverageRating { get; set; } = 0.00m;

        public int? TotalReviews { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

    }
}
