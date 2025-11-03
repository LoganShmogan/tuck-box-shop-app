using TuckBoxApp.Models;

namespace TuckBoxApp.Services;

public class AuthService : IAuthService
{
    private readonly IFirebaseService _firebaseService;
    
    public bool IsAuthenticated { get; private set; }
    public User CurrentUser { get; private set; }
    
    public AuthService(IFirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }
    
    public async Task<(bool success, string message)> RegisterAsync(User user)
    {
        try
        {
            // Check if user already exists
            var existingUsers = await GetAllUsersAsync();
            if (existingUsers.Any(u => u.Email == user.Email))
            {
                return (false, "User with this email already exists");
            }
            
            var success = await _firebaseService.AddUserAsync(user);
            return success ? 
                (true, "Registration successful") : 
                (false, "Registration failed");
        }
        catch (Exception ex)
        {
            return (false, $"Registration error: {ex.Message}");
        }
    }
    
    public async Task<(bool success, string message, User user)> LoginAsync(string email, string password)
    {
        try
        {
            var users = await GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.Email == email && u.Password == password);
            
            if (user != null)
            {
                IsAuthenticated = true;
                CurrentUser = user;
                return (true, "Login successful", user);
            }
            
            return (false, "Invalid email or password", null);
        }
        catch (Exception ex)
        {
            return (false, $"Login error: {ex.Message}", null);
        }
    }
    
    public Task<bool> LogoutAsync()
    {
        IsAuthenticated = false;
        CurrentUser = null;
        return Task.FromResult(true);
    }
    
    public async Task<bool> ChangePasswordAsync(string email, string newPassword)
    {
        try
        {
            var users = await GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.Email == email);
            
            if (user != null)
            {
                user.Password = newPassword;
                return await _firebaseService.UpdateUserAsync(user);
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    private async Task<List<User>> GetAllUsersAsync()
    {
        // This is a simplified approach - in real app, you'd have a proper method in FirebaseService
        try
        {
            // Implementation to get all users
            return new List<User>();
        }
        catch
        {
            return new List<User>();
        }
    }
}