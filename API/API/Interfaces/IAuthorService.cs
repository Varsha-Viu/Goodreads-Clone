using API.DTOs;
using API.Modals;

namespace API.Interfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorsListDto>> GetAuthorsList();
        Task<AuthorsListDto> GetAuthorById(Guid Id);
        Task<bool> AddAuthor(AddAuthorDTO authorDTO);
        Task<bool> UpdateAuthor(Guid authorId, UpdateAuthorDTO updatedAuthorDto);
        Task<bool> DeleteAuthor(Guid authorId);
    }
}
