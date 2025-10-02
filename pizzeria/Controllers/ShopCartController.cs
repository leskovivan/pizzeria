using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pizzeria.Interfaces;

public class ShopCartController : Controller
{
    private readonly IProduct _products;
    private readonly CartRepository _cartRepository;

    public ShopCartController(IProduct products, CartRepository cartRepository)
    {
        _products = products;
        _cartRepository = cartRepository;
    }

    [Route("/cart")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.CategoryId = double.NaN;
        var products = await _cartRepository.GetShopCartItemsAsync();
        return View(products);
    }

    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> AddToCart(int productId, string? returnUrl, int quantity = 1, bool isModal = true)
    {
        var product = await _products.GetProductAsync(productId);
        if (product != null)
        {
            await _cartRepository.AddToCartAsync(product, quantity);
            if (isModal)
                return PartialView("_ConfirmModal", (product.Name, quantity));
        }
        if (returnUrl != null) return Redirect(returnUrl);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> IncrementQuantity(int shopCartItemId)
    {
        var item = await _cartRepository.GetShopCartItemAsync(shopCartItemId);
        if (item != null)
        {
            item.Count += 1;
            await _cartRepository.UpdateFromCartAsync(item);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> DecrementQuantity(int shopCartItemId)
    {
        var item = await _cartRepository.GetShopCartItemAsync(shopCartItemId);
        if (item != null && item.Count > 1)
        {
            item.Count -= 1;
            await _cartRepository.UpdateFromCartAsync(item);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> RemoveFromCart(int shopCartItemId)
    {
        var item = await _cartRepository.GetShopCartItemAsync(shopCartItemId);
        if (item != null) await _cartRepository.RemoveFromCartAsync(item);
        return RedirectToAction(nameof(Index));
    }
}
