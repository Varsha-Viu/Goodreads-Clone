using System.ComponentModel.DataAnnotations;
using API.Modals;

namespace API.DTOs
{
    public class AddBooksDTO
    {
        [Required]
        public string Title { get; set; }
        public string? Subtitle { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string GenreId { get; set; }
        [Required]
        public Guid AuthorId { get; set; }
        public string? PublisherId { get; set; }
        [Required]
        public int PublicationYear { get; set; }
        public IFormFile? CoverImageUrl { get; set; }
        [Required]
        public string Language { get; set; }
        [Required]
        public int PageCount { get; set; }
        public decimal? AverageRating { get; set; } = 0.00m;
        public int? TotalReviews { get; set; } = 0;
    }

    public class BookUpdateDto : AddBooksDTO
    {
        public string? CoverImageFile { get; set; }
    }


    public class BooksListDto
    {
        public string BookId { get; set; }
        public string Title { get; set; }
        public string? Subtitle { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public int PublicationYear { get; set; }
        public string? CoverImageUrl { get; set; }
        public string Language { get; set; }
        public int PageCount { get; set; }
        public decimal? AverageRating { get; set; } = 0.00m;
        public int? TotalReviews { get; set; } = 0;
        public Authors Authors { get; set; }
        public Genre Genre { get; set; }
        public Publishers Publishers { get; set; }
    }
}
