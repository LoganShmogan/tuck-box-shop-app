namespace TuckBoxApp.Models;

public class LocalStore
{
    public Dictionary<string, User> Users { get; set; } = new();
    public Dictionary<string, FoodItem> FoodItems { get; set; } = new();
    public Dictionary<string, City> Cities { get; set; } = new();
    public Dictionary<string, TimeSlot> TimeSlots { get; set; } = new();
    public Dictionary<string, Order> Orders { get; set; } = new();
    public Dictionary<string, DeliveryAddress> DeliveryAddresses { get; set; } = new();
}
