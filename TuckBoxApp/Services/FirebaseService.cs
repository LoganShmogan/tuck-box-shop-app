using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using TuckBoxApp.Models;

namespace TuckBoxApp.Services;

public class FirebaseService : IFirebaseService
{
    private readonly FirebaseClient _firebase;
    private const string BasePath = "tuckbox";
    
    public FirebaseService()
    {
        _firebase = new FirebaseClient("YOUR_FIREBASE_DATABASE_URL",
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult("YOUR_FIREBASE_SECRET")
            });
    }
    
    public async Task<bool> AddUserAsync(User user)
    {
        try
        {
            await _firebase
                .Child(BasePath)
                .Child("users")
                .Child(user.Id)
                .PutAsync(user);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<User> GetUserAsync(string userId)
    {
        try
        {
            return await _firebase
                .Child(BasePath)
                .Child("users")
                .Child(userId)
                .OnceSingleAsync<User>();
        }
        catch
        {
            return null;
        }
    }
    
    public async Task<bool> UpdateUserAsync(User user)
    {
        try
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _firebase
                .Child(BasePath)
                .Child("users")
                .Child(user.Id)
                .PutAsync(user);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> DeleteUserAsync(string userId)
    {
        try
        {
            await _firebase
                .Child(BasePath)
                .Child("users")
                .Child(userId)
                .DeleteAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<List<FoodItem>> GetFoodItemsAsync()
    {
        try
        {
            var items = await _firebase
                .Child(BasePath)
                .Child("foodItems")
                .OnceAsync<FoodItem>();
            
            return items.Select(item => item.Object).ToList();
        }
        catch
        {
            return new List<FoodItem>();
        }
    }
    
    public async Task<List<City>> GetCitiesAsync()
    {
        try
        {
            var cities = await _firebase
                .Child(BasePath)
                .Child("cities")
                .OnceAsync<City>();
            
            return cities.Select(city => city.Object).ToList();
        }
        catch
        {
            return new List<City>();
        }
    }
    
    public async Task<List<TimeSlot>> GetTimeSlotsAsync()
    {
        try
        {
            var slots = await _firebase
                .Child(BasePath)
                .Child("timeSlots")
                .OnceAsync<TimeSlot>();
            
            return slots.Select(slot => slot.Object).ToList();
        }
        catch
        {
            return new List<TimeSlot>();
        }
    }
    
    public async Task<string> AddOrderAsync(Order order)
    {
        try
        {
            var result = await _firebase
                .Child(BasePath)
                .Child("orders")
                .PostAsync(order);
            
            return result.Key;
        }
        catch
        {
            return null;
        }
    }
    
    public async Task<List<Order>> GetUserOrdersAsync(string userId)
    {
        try
        {
            var orders = await _firebase
                .Child(BasePath)
                .Child("orders")
                .OnceAsync<Order>();
            
            return orders
                .Where(o => o.Object.UserId == userId)
                .Select(o => o.Object)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }
        catch
        {
            return new List<Order>();
        }
    }
    
    public async Task<Order> GetOrderAsync(string orderId)
    {
        try
        {
            return await _firebase
                .Child(BasePath)
                .Child("orders")
                .Child(orderId)
                .OnceSingleAsync<Order>();
        }
        catch
        {
            return null;
        }
    }
    
    public async Task<bool> AddDeliveryAddressAsync(DeliveryAddress address)
    {
        try
        {
            await _firebase
                .Child(BasePath)
                .Child("deliveryAddresses")
                .Child(address.Id)
                .PutAsync(address);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<List<DeliveryAddress>> GetUserAddressesAsync(string userId)
    {
        try
        {
            var addresses = await _firebase
                .Child(BasePath)
                .Child("deliveryAddresses")
                .OnceAsync<DeliveryAddress>();
            
            return addresses
                .Where(a => a.Object.UserId == userId)
                .Select(a => a.Object)
                .ToList();
        }
        catch
        {
            return new List<DeliveryAddress>();
        }
    }
    
    public async Task<bool> UpdateDeliveryAddressAsync(DeliveryAddress address)
    {
        try
        {
            await _firebase
                .Child(BasePath)
                .Child("deliveryAddresses")
                .Child(address.Id)
                .PutAsync(address);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> DeleteDeliveryAddressAsync(string addressId)
    {
        try
        {
            await _firebase
                .Child(BasePath)
                .Child("deliveryAddresses")
                .Child(addressId)
                .DeleteAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}