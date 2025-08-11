using Microsoft.EntityFrameworkCore;

namespace BookShoppingCart.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock?> GetStockByBookId(int bookId)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s => s.BookId == bookId);
        }

        public async Task ManageStock(StockDTO stockToManage)
        {
            if (stockToManage is null)
                throw new ArgumentNullException(nameof(stockToManage));

            if (stockToManage.Quantity < 0)
                throw new ArgumentOutOfRangeException(nameof(stockToManage.Quantity), "Quantity cannot be negative.");

            var existingStock = await GetStockByBookId(stockToManage.BookId);

            if (existingStock is null)
            {
                var stock = new Stock
                {
                    BookId = stockToManage.BookId,
                    Quantity = stockToManage.Quantity
                };
                _context.Stocks.Add(stock);
            }
            else
            {
                existingStock.Quantity = stockToManage.Quantity;
                // No need to call Update(); the entity is tracked and SaveChangesAsync will persist the change.
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "")
        {
            var term = (sTerm ?? string.Empty).Trim();

            var query =
                from book in _context.Books.AsNoTracking()
                join stock in _context.Stocks.AsNoTracking()
                    on book.Id equals stock.BookId into book_stock
                from bookStock in book_stock.DefaultIfEmpty()
                where string.IsNullOrEmpty(term)
                      || EF.Functions.Like(book.BookName, $"%{term}%")
                orderby book.BookName
                select new StockDisplayModel
                {
                    BookId = book.Id,
                    BookName = book.BookName,
                    Quantity = bookStock == null ? 0 : bookStock.Quantity
                };

            return await query.ToListAsync();
        }
    }

    public interface IStockRepository
    {
        Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "");
        Task<Stock?> GetStockByBookId(int bookId);
        Task ManageStock(StockDTO stockToManage);
    }
}
