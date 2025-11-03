using TuckBoxApp.Models;
using TuckBoxApp.Services;

namespace TuckBoxApp.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    
    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Register";
    }
    
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _password = string.Empty;
    
    [ObservableProperty]
    private string _confirmPassword = string.Empty;
    
    [ObservableProperty]
    private string _fullName = string.Empty;
    
    [ObservableProperty]
    private string _mobileNumber = string.Empty;
    
    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy) return;
        
        if (string.IsNullOrWhiteSpace(Email) || 
            string.IsNullOrWhiteSpace(Password) || 
            string.IsNullOrWhiteSpace(FullName) || 
            string.IsNullOrWhiteSpace(MobileNumber))
        {
            ShowError("Please fill in all fields");
            return;
        }
        
        if (Password != ConfirmPassword)
        {
            ShowError("Passwords do not match");
            return;
        }
        
        if (Password.Length < 6)
        {
            ShowError("Password must be at least 6 characters");
            return;
        }
        
        IsBusy = true;
        ClearMessages();
        
        var user = new User
        {
            Email = Email,
            Password = Password,
            FullName = FullName,
            MobileNumber = MobileNumber
        };
        
        var (success, message) = await _authService.RegisterAsync(user);
        
        if (success)
        {
            ShowSuccess("Registration successful! Please login.");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            ShowError(message);
        }
        
        IsBusy = false;
    }
    
    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}