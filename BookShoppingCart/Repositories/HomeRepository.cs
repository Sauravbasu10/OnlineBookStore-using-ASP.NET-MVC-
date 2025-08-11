using BookShoppingCart.Data;
using Microsoft.EntityFrameworkCore;

namespace BookShoppingCart.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _context;

        public HomeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Genre>> Genres()
        {
            return await _context.Genres.ToListAsync();
        }
        public async Task<IEnumerable<Book>> GetBooks(string sTerm = "", int genreId = 0)
        {
            sTerm = sTerm.ToLower();

            IEnumerable<Book> books = await (from book in _context.Books
                               join genre in _context.Genres
                               on book.GenreId equals genre.Id
                               join stock in _context.Stocks
                               on book.Id equals stock.BookId
                               into book_stocks
                               from bookWithStock in book_stocks.DefaultIfEmpty()
                               where (string.IsNullOrWhiteSpace(sTerm) || book.BookName.ToLower().StartsWith(sTerm))
                                   
                               select new Book
                               {
                                   Id = book.Id,
                                   BookName = book.BookName,
                                   AuthorName = book.AuthorName,
                                   Price = book.Price,
                                   Image = book.Image,
                                   GenreId = book.GenreId,
                                   GenreName = genre.GenreName,
                                   Quantity = bookWithStock == null ? 0:bookWithStock.Quantity
                               }).ToListAsync();

            if(genreId >0)
            {
                books = books.Where(b => b.GenreId == genreId).ToList();
            }

            return books;
        }
    }
}
