using System.ComponentModel.DataAnnotations;

namespace TuckBoxApp.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Full name is required")]
    public string FullName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Mobile number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string MobileNumber { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}