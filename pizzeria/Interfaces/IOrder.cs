using System.Threading.Tasks;
using pizzeria.Models;

public interface IOrder
{
    PagedList<Order> GetAllOrdersWithDetails(QueryOptions options);
    PagedList<Order> GetAllOrdersByUserWithDetails(QueryOptions options, string userId);
    Task<Order> GetOrderAsync(int id);
    Task AddOrderAsync(Order order);
    Task EditOrderAsync(Order order);
    Task RemoveOrderAsync(Order order);
}
