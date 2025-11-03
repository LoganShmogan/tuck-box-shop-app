using TuckBoxApp.Models;

namespace TuckBoxApp.Services;

public interface IAuthService
{
    Task<(bool success, string message)> RegisterAsync(User user);
    Task<(bool success, string message, User user)> LoginAsync(string email, string password);
    Task<bool> LogoutAsync();
    Task<bool> ChangePasswordAsync(string email, string newPassword);
    bool IsAuthenticated { get; }
    User CurrentUser { get; }
}