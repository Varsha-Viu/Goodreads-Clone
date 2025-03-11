using API.DTOs;
using API.Modals;

namespace API.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BooksListDto>> GetAllBookList();
        Task<IEnumerable<Genre>> GetAllGenreList();
        Task<IEnumerable<Publishers>> GetAllPublishersList();
        Task<bool> AddBooks(AddBooksDTO booksDTO);
        Task<bool> AddGenre(AddGenreDTO genreDTO);
        Task<bool> AddPublishers(AddPublishersDTO publishersDTO);
        Task<bool> UpdateGenreAsync(string genreId, AddGenreDTO genreDto);
        Task<bool> UpdatePublisherAsync(string publisherId, AddPublishersDTO publisherDto);
        Task<bool> DeleteBookAsync(string bookId);
        Task<string> DeletePublisherAsync(string publisherId);
        Task<string> DeleteGenreAsync(string genreId);
        Task<bool> UpdateBookAsync(string bookId, BookUpdateDto bookDto);
    }
}
