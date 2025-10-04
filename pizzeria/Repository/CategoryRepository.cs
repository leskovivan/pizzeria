using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pizzeria.Interfaces;
using pizzeria.Models;

namespace pizzeria.Repository
{
    public class CategoryRepository : ICategory
    {
        private readonly ApplicationContext _db;
        public CategoryRepository(ApplicationContext db) => _db = db;

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
            => await _db.Categories.AsNoTracking().ToListAsync();

        public async Task<Category> GetCategoryAsync(int id)
            => await _db.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddCategoryAsync(Category category)
        {
            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();
        }

        public async Task EditCategoryAsync(Category category)
        {
            _db.Categories.Update(category);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
        }
    }
}
