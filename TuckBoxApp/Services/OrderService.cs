using TuckBoxApp.Models;

namespace TuckBoxApp.Services;

public class OrderService : IOrderService
{
    private readonly IFirebaseService _firebaseService;
    
    public OrderService(IFirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }
    
    public Task<List<Order>> GetUserOrdersAsync(string userId)
    {
        return _firebaseService.GetUserOrdersAsync(userId);
    }
    
    public Task<Order> GetOrderAsync(string orderId)
    {
        return _firebaseService.GetOrderAsync(orderId);
    }
    
    public Task<string> CreateOrderAsync(Order order)
    {
        return _firebaseService.AddOrderAsync(order);
    }
    
    public Task<bool> UpdateOrderAsync(Order order)
    {
        // Implementation would update order in Firebase
        return Task.FromResult(true);
    }
    
    public Task<bool> CancelOrderAsync(string orderId)
    {
        // Implementation would cancel order in Firebase
        return Task.FromResult(true);
    }
}