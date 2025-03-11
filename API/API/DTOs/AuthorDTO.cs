using API.Constants;
using API.Modals;

namespace API.DTOs
{
    public class AddAuthorDTO
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class AuthorsListDto
    {
        public Guid AuthorId { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string PenName { get; set; }
        public string Bio { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public List<AuthorSocialLinks>? SocialLink { get; set; }

    }

    public class UpdateAuthorDTO
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string PenName { get; set; }
        public string Bio { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public List<AddSocialLinkDto>? SocialLink { get; set; }
    }

    public class AddSocialLinkDto
    {
        public SocialLinks Platform { get; set; }
        public string Url { get; set; }
    }
}
