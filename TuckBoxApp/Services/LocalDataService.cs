using Newtonsoft.Json;
using TuckBoxApp.Models;

namespace TuckBoxApp.Services;

public class LocalDataService : ILocalDataService
{
    private const string DataFileName = "tuckbox-data.json";
    private const string SeedFileName = "seed_data.json";

    private readonly SemaphoreSlim _lock = new(1, 1);
    private LocalStore? _store;

    private async Task<LocalStore> GetStoreAsync()
    {
        if (_store != null)
        {
            return _store;
        }

        await _lock.WaitAsync();
        try
        {
            if (_store != null)
            {
                return _store;
            }

            var dataPath = Path.Combine(FileSystem.AppDataDirectory, DataFileName);

            if (!File.Exists(dataPath))
            {
                using var seedStream = await FileSystem.OpenAppPackageFileAsync(SeedFileName);
                using var reader = new StreamReader(seedStream);
                var seedJson = await reader.ReadToEndAsync();
                await File.WriteAllTextAsync(dataPath, seedJson);
            }

            var json = await File.ReadAllTextAsync(dataPath);
            _store = JsonConvert.DeserializeObject<LocalStore>(json) ?? new LocalStore();
            return _store;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task SaveAsync()
    {
        if (_store == null)
        {
            return;
        }

        var dataPath = Path.Combine(FileSystem.AppDataDirectory, DataFileName);
        var json = JsonConvert.SerializeObject(_store, Formatting.Indented);
        await File.WriteAllTextAsync(dataPath, json);
    }

    public async Task<bool> AddUserAsync(User user)
    {
        try
        {
            var store = await GetStoreAsync();
            store.Users[user.Id] = user;
            await SaveAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<User?> GetUserAsync(string userId)
    {
        try
        {
            var store = await GetStoreAsync();
            return store.Users.TryGetValue(userId, out var user) ? user : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        try
        {
            var store = await GetStoreAsync();
            return store.Users.Values.ToList();
        }
        catch
        {
            return new List<User>();
        }
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        try
        {
            user.UpdatedAt = DateTime.UtcNow;
            var store = await GetStoreAsync();
            store.Users[user.Id] = user;
            await SaveAsync();
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
            var store = await GetStoreAsync();
            store.Users.Remove(userId);
            await SaveAsync();
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
            var store = await GetStoreAsync();
            return store.FoodItems.Values.ToList();
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
            var store = await GetStoreAsync();
            return store.Cities.Values.ToList();
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
            var store = await GetStoreAsync();
            return store.TimeSlots.Values.ToList();
        }
        catch
        {
            return new List<TimeSlot>();
        }
    }

    public async Task<string?> AddOrderAsync(Order order)
    {
        try
        {
            var store = await GetStoreAsync();
            store.Orders[order.Id] = order;
            await SaveAsync();
            return order.Id;
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
            var store = await GetStoreAsync();
            return store.Orders.Values
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }
        catch
        {
            return new List<Order>();
        }
    }

    public async Task<Order?> GetOrderAsync(string orderId)
    {
        try
        {
            var store = await GetStoreAsync();
            return store.Orders.TryGetValue(orderId, out var order) ? order : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateOrderAsync(Order order)
    {
        try
        {
            var store = await GetStoreAsync();
            store.Orders[order.Id] = order;
            await SaveAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AddDeliveryAddressAsync(DeliveryAddress address)
    {
        try
        {
            var store = await GetStoreAsync();
            store.DeliveryAddresses[address.Id] = address;
            await SaveAsync();
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
            var store = await GetStoreAsync();
            return store.DeliveryAddresses.Values
                .Where(a => a.UserId == userId)
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
            var store = await GetStoreAsync();
            store.DeliveryAddresses[address.Id] = address;
            await SaveAsync();
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
            var store = await GetStoreAsync();
            store.DeliveryAddresses.Remove(addressId);
            await SaveAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
