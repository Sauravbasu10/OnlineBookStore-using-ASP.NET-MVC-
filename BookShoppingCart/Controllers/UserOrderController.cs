using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCart.Controllers
{
    public class UserOrderController : Controller
    {
        private readonly IUserOrderRepository _userOrderRepository;

        public UserOrderController(IUserOrderRepository userOrderRepository)
        {
           _userOrderRepository = userOrderRepository;
        }
        public async Task<IActionResult> UserOrders()
        {
            var orders = await _userOrderRepository.UserOrders();
            return View(orders);
        }
    }
}
