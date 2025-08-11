using Microsoft.AspNetCore.Identity;

namespace BookShoppingCart.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> AddItem(int bookId, int qty)
        {
            string userId = GetUserId();
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User is not logged in");

                var cart = await GetCart(userId);
                if (cart == null)
                {
                    cart = new ShoppingCart { UserId = userId };
                    await _context.ShoppingCarts.AddAsync(cart);
                    await _context.SaveChangesAsync(); // Generate cart.Id
                }

                var cartItem = await _context.CartDetails
                    .FirstOrDefaultAsync(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);

                if (cartItem != null)
                {
                    cartItem.Quantity += qty; // ✅ Fix: increment quantity
                }
                else
                {
                    var book = _context.Books.Find(bookId);
                    cartItem = new CartDetail
                    {
                        BookId = bookId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = book.Price
                    };
                    await _context.CartDetails.AddAsync(cartItem);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Optional: log or rethrow
                Console.WriteLine("AddItem failed: " + ex.Message);
            }

            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }


        public async Task<int> Remove(int bookId)
        {
            string userId = GetUserId();
            try
            {
                
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("Invalid Cart");

                var cart = await GetCart(userId);
                if (cart == null)
                    throw new Exception("No items in cart");

                var cartItem = await _context.CartDetails
                    .FirstOrDefaultAsync(a => a.ShoppingCartId == cart.Id && a.BookId == bookId);

                if (cartItem == null)
                    throw new Exception("No items in the cart");

                if (cartItem.Quantity == 1)
                {
                    _context.CartDetails.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity -= 1;
                }

                await _context.SaveChangesAsync();
                
            }
            catch (Exception)
            {
                
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new UnauthorizedAccessException("User not found");

            var shoppingCart = await _context.ShoppingCarts
                    .Include(sc => sc.CartDetails)
                    .ThenInclude(cd => cd.Book)
                     .ThenInclude(b => b.Genre)     // path 1: Book → Genre
                    .Include(sc => sc.CartDetails)
                        .ThenInclude(cd => cd.Book)
                          .ThenInclude(b => b.Stock)     // path 2: Book → Stock
     .FirstOrDefaultAsync(sc => sc.UserId == userId);

            return shoppingCart;
        }

        //async Task<ShoppingCart> ICartRepository.GetUserCart()
        //{
        //    return await GetUserCart();
        //}

        private async Task<ShoppingCart> GetCart(string userId)
        {
            return await _context.ShoppingCarts
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }


        public async Task<bool> DoCheckOut(CheckoutModel model)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new UnauthorizedAccessException("User is not logged-in");

                var cart = await GetCart(userId);
                if (cart == null)
                    throw new InvalidOperationException("Invalid User Id");

                var cartDetail = _context.CartDetails
                                    .Where(a => a.ShoppingCartId == cart.Id)
                                    .ToList();

                if (!cartDetail.Any())
                    throw new InvalidOperationException("Cart is Empty");

                var pendingRecord = _context.OrderStatuses.FirstOrDefault(s => s.StatusName == "Pending");
                if (pendingRecord == null)
                    throw new Exception("Pending Order Status not found");

                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.UtcNow,
                    Name = model.Name,
                    Email = model.Email,
                    MobileNumber = model.MobileNumber,
                    Address = model.Address,
                    PaymentMethod = model.PaymentMethod,
                    IsPaid = false,
                    OrderStatusId = pendingRecord.Id
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in cartDetail)
                {
                    var orderDetail = new OrderDetail
                    {
                        BookId = item.BookId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };
                    _context.OrderDetails.Add(orderDetail);

                    var stock = await _context.Stocks.FirstOrDefaultAsync(a => a.BookId == item.BookId);
                    if (stock == null)
                    {
                        throw new InvalidOperationException("Stock is null");
                    }
                    if (item.Quantity > stock.Quantity)
                    {
                        throw new InvalidOperationException($"Only {stock.Quantity} item(s) are available in the stock");

                    }
                    stock.Quantity -= item.Quantity;
                }
               await _context.SaveChangesAsync();

                _context.CartDetails.RemoveRange(cartDetail);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                // Optional: Log exception (ex.Message)
                await transaction.RollbackAsync();
                return false;
            }
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            return _userManager.GetUserId(principal);
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }

            var count = await (from cart in _context.ShoppingCarts
                               join detail in _context.CartDetails on cart.Id equals detail.ShoppingCartId
                               where cart.UserId == userId
                               select detail.Quantity).SumAsync();

            return count;
        }
    }
}
