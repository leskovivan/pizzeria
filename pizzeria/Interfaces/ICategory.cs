using System.Collections.Generic;
using System.Threading.Tasks;
using pizzeria.Models;

namespace pizzeria.Interfaces
{
    public interface ICategory
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryAsync(int id);
        Task AddCategoryAsync(Category category);
        Task EditCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);
    }
}
