using System.Collections.Generic;
using System.Threading.Tasks;
using pizzeria.Models;

namespace pizzeria.Interfaces
{
    public interface IProduct
    {
        PagedList<Product> GetAllProducts(QueryOptions options);
        PagedList<Product> GetAllProductsByCategory(QueryOptions options, int categoryId);
        Task<Product> GetProductAsync(int id);
        Task<Product> GetProductWithCategoryAsync(int id);
        Task<IEnumerable<Product>> GetEightRandomProductsAsync(int productId);
        Task AddProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task EditProductAsync(Product product);
    }
}
