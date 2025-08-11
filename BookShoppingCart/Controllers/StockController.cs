using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCart.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class StockController : Controller
    {
        private readonly IStockRepository _stockRepository;

        public StockController(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        // GET: /Stock
        public async Task<IActionResult> Index(string sTerm = "")
        {
            var stocks = await _stockRepository.GetStocks(sTerm);
            return View(stocks);
        }

        // GET: /Stock/ManageStock/5
        [HttpGet]
        public async Task<IActionResult> ManageStock(int bookId)
        {
            var existingStock = await _stockRepository.GetStockByBookId(bookId);
            var stock = new StockDTO
            {
                BookId = bookId,
                Quantity = existingStock?.Quantity ?? 0
            };
            return View(stock);
        }

        // POST: /Stock/ManageStock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageStock(StockDTO stock)
        {
            if (!ModelState.IsValid)
            {
                // Return the same view to show validation messages
                return View(stock);
            }

            try
            {
                await _stockRepository.ManageStock(stock);
                TempData["successMessage"] = "Stock managed successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["errorMessage"] = "Something went wrong while managing stock.";
                // Optionally add a model error so validation summary shows it
                ModelState.AddModelError(string.Empty, "Unexpected error. Please try again.");
                return View(stock);
            }
        }
    }
}
