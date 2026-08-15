namespace TuckBoxApp.Models;

public class FoodExtraDetails
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FoodItemId { get; set; } = string.Empty;
    public string OptionType { get; set; } = string.Empty;
    public List<string> AvailableChoices { get; set; } = new();
}
