using tuck_box_shop.Models; 

namespace tuck_box_shop.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string email, string password, User userData);
        Task<bool> LogoutAsync();
        string GetCurrentUserId();
        bool IsUserLoggedIn();
    }
}