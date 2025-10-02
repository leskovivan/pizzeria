using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pizzeria.Interfaces;
using pizzeria.Models;

namespace pizzeria.Repository
{
    public class ProductRepository : IProduct
    {
        private readonly ApplicationContext _applicationContext;

        public ProductRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public PagedList<Product> GetAllProducts(QueryOptions options)
            => new PagedList<Product>(_applicationContext.Products.Include(e => e.Category), options);

        public PagedList<Product> GetAllProductsByCategory(QueryOptions options, int categoryId)
            => new PagedList<Product>(_applicationContext.Products
                    .Include(e => e.Category)
                    .Where(e => e.CategoryId == categoryId),
                options);

        public async Task AddProductAsync(Product product)
        {
            await _applicationContext.Products.AddAsync(product);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            _applicationContext.Products.Remove(product);
            await _applicationContext.SaveChangesAsync();
        }

        public async Task EditProductAsync(Product product)
        {
            _applicationContext.Entry(product).State = EntityState.Modified;
            _applicationContext.Entry(product).Property(e => e.DateOfPublication).IsModified = false;
            await _applicationContext.SaveChangesAsync();
        }

        public async Task<Product> GetProductAsync(int id)
            => await _applicationContext.Products.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

        public async Task<Product> GetProductWithCategoryAsync(int id)
            => await _applicationContext.Products.Include(e => e.Category)
                .AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

        public async Task<IEnumerable<Product>> GetEightRandomProductsAsync(int productId)
            => await _applicationContext.Products
                .Where(e => e.Id != productId)
                .OrderBy(_ => Guid.NewGuid())
                .Take(8)
                .ToListAsync();
    }
}
