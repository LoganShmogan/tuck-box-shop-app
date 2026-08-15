using TuckBoxApp.Models;

namespace TuckBoxApp.Services;

public class AuthService : IAuthService
{
    private readonly ILocalDataService _localDataService;

    public bool IsAuthenticated { get; private set; }
    public User CurrentUser { get; private set; }

    public AuthService(ILocalDataService localDataService)
    {
        _localDataService = localDataService;
    }

    public async Task<(bool success, string message)> RegisterAsync(User user)
    {
        try
        {
            var existingUsers = await _localDataService.GetAllUsersAsync();
            if (existingUsers.Any(u => u.Email == user.Email))
            {
                return (false, "User with this email already exists");
            }

            user.Password = PasswordHasher.Hash(user.Password);

            var success = await _localDataService.AddUserAsync(user);
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
            var hashedPassword = PasswordHasher.Hash(password);
            var users = await _localDataService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.Email == email && u.Password == hashedPassword);

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
            var users = await _localDataService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                user.Password = PasswordHasher.Hash(newPassword);
                return await _localDataService.UpdateUserAsync(user);
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}
