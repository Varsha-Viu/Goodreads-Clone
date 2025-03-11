using System.Security.Claims;
using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpPost("get-all-genres")]
        public async Task<IActionResult> GetGenresList()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await _bookService.GetAllGenreList();
            return Ok(result);
        }

        [HttpPost("get-all-publishers")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> GetPublishersList()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await _bookService.GetAllPublishersList();
            return Ok(result);
        }

        [HttpPost("get-all-books")]
        public async Task<IActionResult> GetBooksList()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await _bookService.GetAllBookList();
            return Ok(result);
        }

        [HttpPost("add-books")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> AddBooks(AddBooksDTO booksDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await _bookService.AddBooks(booksDTO);
            if (result)
                return Ok(new ResponseDto("Book added successfully", true));
            return NotFound(new ResponseDto("Error while adding book", false));
        }

        [HttpPost("add-publishers")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> AddPublishers(AddPublishersDTO  publishersDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await _bookService.AddPublishers(publishersDTO);
            if (result)
                return Ok(new ResponseDto("Publisher added successfully", true));
            return NotFound(new ResponseDto("Error while adding publisher", false));
        }

        [HttpPost("add-generes")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> AddGeneres(AddGenreDTO genreDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await _bookService.AddGenre(genreDTO);

            if (result)
                return Ok(new ResponseDto("Genre added successfully", true));
            return NotFound(new ResponseDto("Error while adding genre", false));
        }

        [HttpPut("update-books/{bookId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdateBook(string bookId, [FromForm] BookUpdateDto bookDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isUpdated = await _bookService.UpdateBookAsync(bookId, bookDto);
            if (!isUpdated)
            {
                return NotFound(new { Message = "Book not found" });
            }

            return Ok(new { Message = "Book updated successfully!", CoverImageUrl = bookDto.CoverImageUrl });
        }

        [HttpPut("update-genre/{genreId}")]
        public async Task<IActionResult> UpdateGenre(string genreId, [FromBody] AddGenreDTO genreDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isUpdated = await _bookService.UpdateGenreAsync(genreId, genreDto);
            if (!isUpdated)
            {
                return NotFound(new { Message = "Genre not found" });
            }

            return Ok(new { Message = "Genre updated successfully!" });
        }

        [HttpPut("update/{publisherId}")]
        public async Task<IActionResult> UpdatePublisher(string publisherId, [FromBody] AddPublishersDTO publisherDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isUpdated = await _bookService.UpdatePublisherAsync(publisherId, publisherDto);
            if (!isUpdated)
            {
                return NotFound(new { Message = "Publisher not found" });
            }

            return Ok(new { Message = "Publisher updated successfully!" });
        }

        [HttpDelete("delete-genre/{genreId}")]
        public async Task<IActionResult> DeleteGenre(string genreId)
        {
            try
            {
                string isDeleted = await _bookService.DeleteGenreAsync(genreId);
                if (!string.IsNullOrEmpty(isDeleted))
                {
                    return NotFound(new ResponseDto(isDeleted, false));
                }

                return Ok(new { Message = "Genre deleted successfully!" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("delete-publisher/{publisherId}")]
        public async Task<IActionResult> DeletePublisher(string publisherId)
        {
            try
            {
                string isDeleted = await _bookService.DeletePublisherAsync(publisherId);
                if (!string.IsNullOrEmpty(isDeleted))
                {
                    return NotFound(new ResponseDto(isDeleted, false));
                }

                return Ok(new { Message = "Publisher deleted successfully!" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("delete-book/{bookId}")]
        public async Task<IActionResult> DeleteBook(string bookId)
        {
            bool isDeleted = await _bookService.DeleteBookAsync(bookId);
            if (!isDeleted)
            {
                return NotFound(new { Message = "Book not found" });
            }

            return Ok(new { Message = "Book deleted successfully!" });
        }
    }
}
