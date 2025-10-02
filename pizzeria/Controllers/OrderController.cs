using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using pizzeria.ViewModels;

public class OrderController : Controller
{
    private readonly CartRepository _cartRepository;
    private readonly IOrder _orders;
    private readonly UserManager<User> _userManager;

    public OrderController(CartRepository cartRepository, IOrder orders, UserManager<User> userManager)
    {
        _cartRepository = cartRepository;
        _orders = orders;
        _userManager = userManager;
    }

    [Route("order-info")]
    [HttpPost]
    public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return NotFound();
            var currentUser = await _userManager.FindByIdAsync(userId);
            return View("Authenticated", new OrderViewModelAuthenticated
            {
                City = currentUser.City,
                Address = currentUser.Address
            });
        }
        else
        {
            return View("NonAuthenticated");
        }
    }

    [Route("order-finish-result")]
    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> FinishOrderNonAuthenticated(OrderViewModel vm)
    {
        if (!ModelState.IsValid) return View("NonAuthenticated", vm);

        var order = new Order
        {
            Fio = vm.Fio,
            Email = vm.Email,
            Phone = vm.Phone,
            City = vm.City,
            Address = vm.Address
        };

        var products = await _cartRepository.GetShopCartItemsAsync();
        order.OrderDetails = products.Select(e => new OrderDetails
        {
            Order = order,
            ProductId = e.ProductId,
            Quantity = e.Count
        }).ToList();

        await _orders.AddOrderAsync(order);
        await _cartRepository.ClearCartAsync();
        return View("ThankYou");
    }

    [Route("order-finish")]
    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> FinishOrder(OrderViewModelAuthenticated vm)
    {
        if (!ModelState.IsValid) return View("Authenticated", vm);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return NotFound();

        var currentUser = await _userManager.FindByIdAsync(userId);
        var order = new Order
        {
            UserId = userId,
            City = vm.City,
            Address = vm.Address
        };

        if (!string.Equals(currentUser.City, vm.City, System.StringComparison.OrdinalIgnoreCase))
        {
            currentUser.City = vm.City;
            await _userManager.UpdateAsync(currentUser);
        }
        if (!string.Equals(currentUser.Address, vm.Address, System.StringComparison.OrdinalIgnoreCase))
        {
            currentUser.Address = vm.Address;
            await _userManager.UpdateAsync(currentUser);
        }

        var products = await _cartRepository.GetShopCartItemsAsync();
        order.OrderDetails = products.Select(e => new OrderDetails
        {
            Order = order,
            ProductId = e.ProductId,
            Quantity = e.Count
        }).ToList();

        await _orders.AddOrderAsync(order);
        await _cartRepository.ClearCartAsync();
        return View("ThankYou");
    }

    [Route("orders")]
    [HttpGet]
    public IActionResult MyOrders(QueryOptions options)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return NotFound();
        return View(_orders.GetAllOrdersByUserWithDetails(options, userId));
    }

    [Authorize(Roles = "Admin")]
    [Route("/panel-orders")]
    [HttpGet]
    public IActionResult Orders(QueryOptions options)
        => View(_orders.GetAllOrdersWithDetails(options));

    [Authorize(Roles = "Admin")]
    [Route("/panel/delete-order")]
    [HttpDelete]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        var currentOrder = await _orders.GetOrderAsync(orderId);
        if (currentOrder != null) await _orders.RemoveOrderAsync(currentOrder);
        return Ok();
    }
}
