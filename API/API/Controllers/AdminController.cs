using System.Security.Claims;
using API.DTOs;
using API.Interfaces;
using API.Modals;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly IAdminService _admin;
        public AdminController(UserManager<Users> userManager, IAdminService admin)
        {
            _userManager = userManager;
            _admin = admin;
        }

        [HttpPost("add-author")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> AddAuthor([FromForm] AuthorDTO author)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var userByEmail = await _userManager.FindByEmailAsync(author.Email);
            if (userByEmail != null)
                return NotFound(new ResponseDto("User with email already exists", false));

            var result = await _admin.AddAuthor(author);
            if(result) 
                return Ok(new ResponseDto("Author added successfully", true));

            return NotFound(new ResponseDto("Error while adding author", false));
        }
    }
}
