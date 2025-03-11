using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class AddGenreDTO
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
