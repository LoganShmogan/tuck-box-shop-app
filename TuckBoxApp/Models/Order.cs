namespace TuckBoxApp.Models;

public class Order
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string DeliveryAddressId { get; set; } = string.Empty;
    public string CityId { get; set; } = string.Empty;
    public string TimeSlotId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public string Notes { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    public string FoodItemId { get; set; } = string.Empty;
    public string FoodItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Customization { get; set; } = string.Empty;
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Preparing,
    OutForDelivery,
    Delivered,
    Cancelled
}