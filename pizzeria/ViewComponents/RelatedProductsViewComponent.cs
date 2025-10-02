using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pizzeria.Interfaces;

public class RelatedProductsViewComponent : ViewComponent
{
    private readonly IProduct _products;
    public RelatedProductsViewComponent(IProduct products) => _products = products;
    public async Task<IViewComponentResult> InvokeAsync(int productId)
        => View("RelatedProducts", await _products.GetEightRandomProductsAsync(productId));
}
