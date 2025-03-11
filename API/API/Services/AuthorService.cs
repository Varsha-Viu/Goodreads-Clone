using API.Data;
using API.DTOs;
using API.Interfaces;
using API.Modals;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly UserManager<Users> _userManager;
        private readonly ApplicationDbContext _db;
        public AuthorService(UserManager<Users> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IEnumerable<AuthorsListDto>> GetAuthorsList()
        {
            var authors = await _db.Authors.ToListAsync();
            var authorsList = new List<AuthorsListDto>();

            foreach (var author in authors)
            {
                var user = await _db.Users.FirstOrDefaultAsync(m => m.Id == author.UserId);
                var social = await _db.AuthorSocialLinks.Where(m => m.AuthorId == author.AuthorId).ToListAsync();

                if (user != null)
                {
                    authorsList.Add(new AuthorsListDto
                    {
                        AuthorId = author.AuthorId,
                        UserId = author.UserId,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserName = user.UserName,
                        PenName = author.PenName,
                        Bio = author.Bio,
                        Address = user.Address,
                        PhoneNumber = user.PhoneNumber,
                        SocialLink = social.Select(s => new AuthorSocialLinks
                        {
                            SocialId = s.SocialId,
                            Platform = s.Platform,
                            Url = s.Url
                        }).ToList()
                    });
                }
            }

            return authorsList;
        }

        public async Task<AuthorsListDto> GetAuthorById(Guid Id)
        {
            var author = await _db.Authors.FindAsync(Id);
            if(author != null)
            {
                var user = await _db.Users.FirstOrDefaultAsync(m => m.Id == author.UserId);
                var social = await _db.AuthorSocialLinks.Where(m => m.AuthorId == author.AuthorId).ToListAsync();

                if (user != null)
                {
                    var authorById = new AuthorsListDto
                    {
                        AuthorId = author.AuthorId,
                        UserId = author.UserId,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserName = user.UserName,
                        PenName = author.PenName,
                        Bio = author.Bio,
                        Address = user.Address,
                        PhoneNumber = user.PhoneNumber,
                        SocialLink = social.Select(s => new AuthorSocialLinks
                        {
                            SocialId = s.SocialId,
                            Platform = s.Platform,
                            Url = s.Url
                        }).ToList()
                    };

                    return authorById;
                }
            }
            

            return new AuthorsListDto();
        }

        public async Task<bool>  AddAuthor(AddAuthorDTO authorDTO)
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

            var password = authorDTO.FirstName + "@1234";
            var result = await _userManager.CreateAsync(user, password); // first three authors pawd - "Test@1234"
            if (result.Succeeded)
            {
                
                var currentUser = await _userManager.FindByEmailAsync(authorDTO.Email);
                if (currentUser is not null)
                {
                    // add role
                    var addRole = await _userManager.AddToRoleAsync(currentUser, "Author");

                    // add author in authors table
                    var author = new Authors
                    {
                        UserId = currentUser.Id
                    };
                    var authorResult = await _db.AddAsync(author);
                    await _db.SaveChangesAsync();

                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateAuthor(Guid authorId, UpdateAuthorDTO updatedAuthorDto)
        {
            var author = await _db.Authors.FirstOrDefaultAsync(a => a.AuthorId == authorId);
            if (author == null)
                return false; // Author not found

            // Update author fields
            author.PenName = updatedAuthorDto.PenName;
            author.Bio = updatedAuthorDto.Bio;

            // Fetch user linked to this author
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == author.UserId);
            if (user != null)
            {
                user.Email = updatedAuthorDto.Email;
                user.FirstName = updatedAuthorDto.FirstName;
                user.LastName = updatedAuthorDto.LastName;
                user.UserName = updatedAuthorDto.UserName;
                user.Address = updatedAuthorDto.Address;
                user.PhoneNumber = updatedAuthorDto.PhoneNumber;
            }

            // Fetch all existing social links for the author
            var existingSocialLinks = await _db.AuthorSocialLinks
                .Where(m => m.AuthorId == authorId)
                .ToListAsync();

            // If social links exist, update them; otherwise, add new ones
            foreach (var item in updatedAuthorDto.SocialLink)
            {
                var existingLink = existingSocialLinks.FirstOrDefault(s => s.Platform == item.Platform.ToString());

                if (existingLink != null)
                {
                    // Update existing social link
                    existingLink.Url = item.Url;
                    _db.AuthorSocialLinks.Update(existingLink);
                }
                else
                {
                    // Add new social link
                    var newLink = new AuthorSocialLinks
                    {
                        AuthorId = authorId,
                        Platform = item.Platform.ToString(),
                        Url = item.Url
                    };
                    await _db.AuthorSocialLinks.AddAsync(newLink);
                }
            }

            // Save changes to database
            //await _db.SaveChangesAsync();


            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAuthor(Guid authorId)
        {
            var author = await _db.Authors.FirstOrDefaultAsync(a => a.AuthorId == authorId);
            if (author == null)
            {
                return false; // Author not found
            }

            // Find the associated user
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == author.UserId);
            if (user != null)
            {
                _db.Users.Remove(user); // Delete user first
            }

            var social = await _db.AuthorSocialLinks.Where(m => m.AuthorId == authorId).ToListAsync();

            if (social != null)
            {
                _db.AuthorSocialLinks.RemoveRange(social); //remove social links
            }

            _db.Authors.Remove(author); // Then delete author
            await _db.SaveChangesAsync(); // Save both deletions

            return true;
        }
    }
}
