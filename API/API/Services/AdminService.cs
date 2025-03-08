using API.Data;
using API.DTOs;
using API.Interfaces;
using API.Modals;
using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<Users> _userManager;
        private readonly ApplicationDbContext _db;
        public AdminService(UserManager<Users> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        public async Task<bool> AddAuthor(AuthorDTO authorDTO)
        {
            var user = new Users
            {
                Email = authorDTO.Email,
                UserName = authorDTO.Email,
                FirstName = authorDTO.FirstName,
                LastName = authorDTO.LastName,
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = false,
                LockoutEnd = DateTime.Now
            };
            var result = await _userManager.CreateAsync(user, "Test@1234");
            if(result.Succeeded)
            {
                
                var currentUser = await _userManager.FindByEmailAsync(authorDTO.Email);
                if (currentUser is not null)
                {
                    // add role
                    var addRole = await _userManager.AddToRoleAsync(currentUser, "Author");

                    // add author in authors table
                    var author = new Authors
                    {
                        UserId = new Guid(currentUser.Id)
                    };
                    var authorResult = await _db.AddAsync(author);
                    await _db.SaveChangesAsync();

                    return true;
                }
            }

            return false;
        }
    }
}
