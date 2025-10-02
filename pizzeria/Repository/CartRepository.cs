using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using pizzeria.Models;

public class CartRepository : ICart
{
    private readonly ApplicationContext _applicationContext;
    public CartRepository(ApplicationContext applicationContext) => _applicationContext = applicationContext;
    public string ShopCartId { get; set; }

    public static CartRepository GetCart(IServiceProvider service)
    {
        ISession session = service.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
        var context = service.GetService<ApplicationContext>();
        string shopCartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
        session.SetString("CartId", shopCartId);
        return new CartRepository(context) { ShopCartId = shopCartId };
    }

    public async Task AddToCartAsync(Product product, int quantity)
    {
        await _applicationContext.ShopCartItems.AddAsync(new ShopCartItem
        {
            ShopCartId = ShopCartId,
            ProductId = product.Id,
            Price = product.Price,
            Count = quantity
        });
        await _applicationContext.SaveChangesAsync();
    }

    public async Task<int> GetShopCartItemsCountAsync()
        => await _applicationContext.ShopCartItems.Where(e => e.ShopCartId == ShopCartId).CountAsync();

    public async Task<IEnumerable<ShopCartItem>> GetShopCartItemsAsync()
        => await _applicationContext.ShopCartItems
            .Where(e => e.ShopCartId == ShopCartId)
            .Include(e => e.Product)
            .ToListAsync();

    public async Task<ShopCartItem> GetShopCartItemAsync(int shopCartItemId)
        => await _applicationContext.ShopCartItems.FirstOrDefaultAsync(e => e.Id == shopCartItemId);

    public async Task RemoveFromCartAsync(ShopCartItem shopCartItem)
    {
        _applicationContext.ShopCartItems.Remove(shopCartItem);
        await _applicationContext.SaveChangesAsync();
    }

    public async Task UpdateFromCartAsync(ShopCartItem shopCartItem)
    {
        _applicationContext.ShopCartItems.Update(shopCartItem);
        await _applicationContext.SaveChangesAsync();
    }

    public async Task ClearCartAsync()
        => await _applicationContext.ShopCartItems
            .Where(e => e.ShopCartId == ShopCartId)
            .ExecuteDeleteAsync();
}
