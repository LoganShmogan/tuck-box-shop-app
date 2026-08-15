using TuckBoxApp.Models;

namespace TuckBoxApp.Services;

public class OrderService : IOrderService
{
    private readonly ILocalDataService _localDataService;

    public OrderService(ILocalDataService localDataService)
    {
        _localDataService = localDataService;
    }

    public Task<List<Order>> GetUserOrdersAsync(string userId)
    {
        return _localDataService.GetUserOrdersAsync(userId);
    }

    public Task<Order> GetOrderAsync(string orderId)
    {
        return _localDataService.GetOrderAsync(orderId);
    }

    public Task<string> CreateOrderAsync(Order order)
    {
        return _localDataService.AddOrderAsync(order);
    }

    public Task<bool> UpdateOrderAsync(Order order)
    {
        return _localDataService.UpdateOrderAsync(order);
    }

    public async Task<bool> CancelOrderAsync(string orderId)
    {
        var order = await _localDataService.GetOrderAsync(orderId);
        if (order == null)
        {
            return false;
        }

        order.Status = OrderStatus.Cancelled;
        return await _localDataService.UpdateOrderAsync(order);
    }
}
