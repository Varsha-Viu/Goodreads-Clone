using API.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using API.Modals;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly IAuthorService _authorService;
        public AuthorController(UserManager<Users> userManager, IAuthorService authorService)
        {
            _userManager = userManager;
            _authorService = authorService;
        }

        [HttpGet("get-all-authors")]
        public async Task<IActionResult> GetAllAuthorsList()
        {
            var result = await _authorService.GetAuthorsList();
            return Ok(result);
        }

        [HttpGet("get-author/{authorId}")]
        public async Task<IActionResult> GetAuthorById(Guid authorId)
        {
            var result = await _authorService.GetAuthorById(authorId);

            if (result.UserId == null)
                return NotFound(new ResponseDto("No author found", false));
            
            return Ok(result);
        }

        [HttpPost("add-author")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> AddAuthor([FromForm] AddAuthorDTO author)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var userByEmail = await _userManager.FindByEmailAsync(author.Email);
            if (userByEmail != null)
                return NotFound(new ResponseDto("User with email already exists", false));

            var result = await _authorService.AddAuthor(author);
            if (result)
                return Ok(new ResponseDto("Author added successfully", true));

            return NotFound(new ResponseDto("Error while adding author", false));
        }

        [HttpPost("update-author")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdateAuthor(Guid authorId, [FromBody] UpdateAuthorDTO updatedAuthorDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            if (updatedAuthorDto == null || authorId == Guid.Empty)
            {
                return BadRequest("Invalid data.");
            }

            var result = await _authorService.UpdateAuthor(authorId, updatedAuthorDto);

            if (result)
                return Ok(new ResponseDto("Author details updated successfully", true));

            return NotFound(new ResponseDto("Error while updating author details", false));
        }

        [HttpDelete("{authorId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteAuthor(Guid authorId)
        {
            var isDeleted = await _authorService.DeleteAuthor(authorId);
            if (!isDeleted)
            {
                return NotFound("Author not found.");
            }

            return Ok("Author deleted successfully.");
        }
    }
}
