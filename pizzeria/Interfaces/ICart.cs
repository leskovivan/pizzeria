using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICart
{
    string ShopCartId { get; set; }
    Task<int> GetShopCartItemsCountAsync();
    Task<IEnumerable<ShopCartItem>> GetShopCartItemsAsync();
    Task<ShopCartItem> GetShopCartItemAsync(int shopCartItemId);
    Task AddToCartAsync(pizzeria.Models.Product product, int quantity);
    Task RemoveFromCartAsync(ShopCartItem shopCartItem);
    Task UpdateFromCartAsync(ShopCartItem shopCartItem);
    Task ClearCartAsync();
}
