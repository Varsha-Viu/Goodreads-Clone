using Microsoft.AspNetCore.Identity;

namespace API.Modals
{
    public class Users: IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
    }
}
