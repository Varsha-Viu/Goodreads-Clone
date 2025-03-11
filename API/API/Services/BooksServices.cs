using API.Data;
using API.DTOs;
using API.Interfaces;
using API.Modals;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class BooksServices : IBookService
    {
        private readonly string _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "/BookCovers/UploadedImages");
        private readonly ApplicationDbContext _db;
        public BooksServices(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public async Task<bool> AddBooks(AddBooksDTO booksDTO)
        {
            //upload image
            var imgUrl = "";
            if(booksDTO.CoverImageUrl != null)
            {
                imgUrl = await UploadImage(booksDTO.CoverImageUrl);

                if (string.IsNullOrEmpty(imgUrl))
                    return false;
            }
            

            var books = new Books
            {
                BookId = Guid.NewGuid().ToString(),
                Title = booksDTO.Title,
                Subtitle = booksDTO.Subtitle,
                Description = booksDTO.Description,
                AuthorId = booksDTO.AuthorId,
                ISBN = booksDTO.ISBN,
                GenreId = booksDTO.GenreId,
                PublisherId = booksDTO.PublisherId,
                PublicationYear = booksDTO.PublicationYear,
                Language = booksDTO.Language,
                PageCount = booksDTO.PageCount,
                CoverImageUrl = imgUrl,
                AverageRating = booksDTO.AverageRating,
                TotalReviews = booksDTO.TotalReviews,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var authorResult = await _db.AddAsync(books);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddGenre(AddGenreDTO genreDTO)
        {
            var genre = new Genre
            {
                GenreId = Guid.NewGuid().ToString(),
                Name = genreDTO.Name,
                Description = genreDTO.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var isAdded = await _db.AddAsync(genre);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddPublishers(AddPublishersDTO publishersDTO)
        {
            var publishers = new Publishers
            {
                PublisherId = Guid.NewGuid().ToString(),
                Email = publishersDTO.Email,
                Address = publishersDTO.Address,
                Name = publishersDTO.Name,
                PhoneNumber = publishersDTO.PhoneNumber,
                WebsiteUrl = publishersDTO.WebsiteUrl,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var isAdded = await _db.AddAsync(publishers);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<BooksListDto>> GetAllBookList()
        {
            var books = await _db.Books.ToListAsync();
            var booksList = new List<BooksListDto>();

            foreach (var book in books)
            {
                var genre = await _db.Genre.FirstOrDefaultAsync(m => m.GenreId == book.GenreId);
                var publishers = await _db.Publishers.FirstOrDefaultAsync(m => m.PublisherId == book.PublisherId);
                var author = await _db.Authors.FirstOrDefaultAsync(m => m.AuthorId == book.AuthorId);

                booksList.Add(new BooksListDto
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Subtitle = book.Subtitle,
                    Description = book.Description,
                    ISBN = book.ISBN,
                    PublicationYear = book.PublicationYear,
                    CoverImageUrl = book.CoverImageUrl,
                    Language = book.Language,
                    PageCount = book.PageCount,
                    AverageRating = book.AverageRating,
                    TotalReviews = book.TotalReviews,
                    Authors = author,
                    Publishers = publishers,
                    Genre = genre
                });
            }
            return booksList;
        }

        public async Task<IEnumerable<Genre>> GetAllGenreList()
        {
            var genresList = await _db.Genre.ToListAsync();
            return genresList;
        }

        public async Task<IEnumerable<Publishers>> GetAllPublishersList()
        {
            var publishersList = await _db.Publishers.ToListAsync();
            return publishersList;
        }

        public async Task<bool> UpdateBookAsync(string bookId, BookUpdateDto bookDto)
        {
            var book = await _db.Books.FindAsync(bookId);
            if (book == null)
            {
                return false; // Book not found
            }

            // Update book details
            book.Title = bookDto.Title;
            book.Subtitle = bookDto.Subtitle;
            book.Description = bookDto.Description;
            book.ISBN = bookDto.ISBN;
            book.GenreId = bookDto.GenreId;
            book.AuthorId = bookDto.AuthorId;
            book.PublisherId = bookDto.PublisherId;
            book.PublicationYear = bookDto.PublicationYear;
            book.Language = bookDto.Language;
            book.PageCount = bookDto.PageCount;
            book.AverageRating = bookDto.AverageRating ?? book.AverageRating;
            book.TotalReviews = bookDto.TotalReviews ?? book.TotalReviews;
            book.UpdatedAt = DateTime.UtcNow;

            // Handle cover image upload if a new file is provided
            if (bookDto.CoverImageUrl != null)
            {
                var imgUrl = await UploadImage(bookDto.CoverImageUrl);

                // Update the CoverImageUrl field in the database
                book.CoverImageUrl = imgUrl;
            }

            // Save changes
            _db.Books.Update(book);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateGenreAsync(string genreId, AddGenreDTO genreDto)
        {
            var genre = await _db.Genre.FindAsync(genreId);
            if (genre == null)
            {
                return false; // Genre not found
            }

            // Update genre properties
            genre.Name = genreDto.Name;
            genre.Description = genreDto.Description;

            // Save changes
            _db.Genre.Update(genre);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdatePublisherAsync(string publisherId, AddPublishersDTO publisherDto)
        {
            var publisher = await _db.Publishers.FindAsync(publisherId);
            if (publisher == null)
            {
                return false; // Publisher not found
            }

            // Update publisher properties
            publisher.Name = publisherDto.Name;
            publisher.Email = publisherDto.Email;
            publisher.Address = publisherDto.Address;
            publisher.PhoneNumber = publisherDto.PhoneNumber;
            publisher.WebsiteUrl = publisherDto.WebsiteUrl;

            // Save changes
            _db.Publishers.Update(publisher);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteBookAsync(string bookId)
        {
            var book = await _db.Books.FindAsync(bookId);
            if (book == null)
            {
                return false; // Book not found
            }

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<string> DeletePublisherAsync(string publisherId)
        {
            var publisher = await _db.Publishers.FindAsync(publisherId);
            if (publisher == null)
            {
                return "Publisher not found"; // Publisher not found
            }

            // Check if any books exist under this publisher
            bool hasBooks = await _db.Books.AnyAsync(b => b.PublisherId == publisherId.ToString());
            if (hasBooks)
            {
                return "Cannot delete publisher. Books exist under this publisher.";
            }

            _db.Publishers.Remove(publisher);
            await _db.SaveChangesAsync();

            return "";
        }

        public async Task<string> DeleteGenreAsync(string genreId)
        {
            var genre = await _db.Genre.FindAsync(genreId);
            if (genre == null)
            {
                return "Genre not found"; // Genre not found
            }

            // Check if any books exist under this genre
            bool hasBooks = await _db.Books.AnyAsync(b => b.GenreId == genreId.ToString());
            if (hasBooks)
            {
                return "Cannot delete genre. Books exist under this genre.";
            }

            _db.Genre.Remove(genre);
            await _db.SaveChangesAsync();

            return "";
        }

        private async Task<string> UploadImage(IFormFile coverImg)
        {
            try
            {
                // Ensure the folder exists
                if (!Directory.Exists(_uploadFolder))
                {
                    Directory.CreateDirectory(_uploadFolder);
                }

                // Generate a unique filename
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(coverImg.FileName)}";
                string filePath = Path.Combine(_uploadFolder, fileName);

                // Save the file to the folder
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await coverImg.CopyToAsync(stream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while uploading file: {ex}");
            }
            return "";
        }
    }
}
