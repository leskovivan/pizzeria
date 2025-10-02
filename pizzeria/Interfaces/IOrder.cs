using System.Threading.Tasks;

public interface IOrder
{
    PagedList<Order> GetAllOrdersWithDetails(QueryOptions options);
    PagedList<Order> GetAllOrdersByUserWithDetails(QueryOptions options, string userId);
    Task<Order> GetOrderAsync(int id);
    Task AddOrderAsync(Order order);
    Task EditOrderAsync(Order order);
    Task RemoveOrderAsync(Order order);
}
