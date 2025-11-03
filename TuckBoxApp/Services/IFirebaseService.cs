using TuckBoxApp.Models;

namespace TuckBoxApp.Services;

public interface IFirebaseService
{
    Task<bool> AddUserAsync(User user);
    Task<User> GetUserAsync(string userId);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(string userId);
    
    Task<List<FoodItem>> GetFoodItemsAsync();
    Task<List<City>> GetCitiesAsync();
    Task<List<TimeSlot>> GetTimeSlotsAsync();
    
    Task<string> AddOrderAsync(Order order);
    Task<List<Order>> GetUserOrdersAsync(string userId);
    Task<Order> GetOrderAsync(string orderId);
    
    Task<bool> AddDeliveryAddressAsync(DeliveryAddress address);
    Task<List<DeliveryAddress>> GetUserAddressesAsync(string userId);
    Task<bool> UpdateDeliveryAddressAsync(DeliveryAddress address);
    Task<bool> DeleteDeliveryAddressAsync(string addressId);
}