using TuckBoxApp.Models;

namespace TuckBoxApp.Services;

public interface IOrderService
{
    Task<List<Order>> GetUserOrdersAsync(string userId);
    Task<Order> GetOrderAsync(string orderId);
    Task<string> CreateOrderAsync(Order order);
    Task<bool> UpdateOrderAsync(Order order);
    Task<bool> CancelOrderAsync(string orderId);
}