namespace TuckBoxApp.Models;

public class FoodItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public List<string> CustomizationOptions { get; set; } = new();
}

public class FoodExtraDetails
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FoodItemId { get; set; } = string.Empty;
    public string OptionType { get; set; } = string.Empty;
    public List<string> AvailableChoices { get; set; } = new();
}