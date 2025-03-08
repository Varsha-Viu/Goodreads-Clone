using API.DTOs;

namespace API.Interfaces
{
    public interface IAdminService
    {
        Task<bool> AddAuthor(AuthorDTO authorDTO);
    }
}
