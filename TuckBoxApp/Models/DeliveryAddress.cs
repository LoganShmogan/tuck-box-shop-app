namespace TuckBoxApp.Models;

public class DeliveryAddress
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Address line is required")]
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "City is required")]
    public string City { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Postal code is required")]
    public string PostalCode { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public string Label { get; set; } = "Home"; // Home, Work, etc.
}