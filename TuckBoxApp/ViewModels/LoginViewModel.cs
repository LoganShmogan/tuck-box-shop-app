using TuckBoxApp.Services;
using TuckBoxApp.Views;

namespace TuckBoxApp.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    
    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Login";
    }
    
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _password = string.Empty;
    
    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;
        
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ShowError("Please enter both email and password");
            return;
        }
        
        IsBusy = true;
        ClearMessages();
        
        var (success, message, user) = await _authService.LoginAsync(Email, Password);
        
        if (success)
        {
            ShowSuccess("Login successful!");
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        else
        {
            ShowError(message);
        }
        
        IsBusy = false;
    }
    
    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }
}