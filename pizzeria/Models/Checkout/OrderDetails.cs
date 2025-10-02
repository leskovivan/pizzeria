public class OrderDetails
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int ProductId { get; set; }
    public pizzeria.Models.Product Product { get; set; }
    public int Quantity { get; set; }
}
