using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using tuck_box_shop.Services;

namespace tuck_box_shop.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        private readonly IAuthService _authService;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter both email and password", "OK");
                return;
            }

            bool isSuccess = await _authService.LoginAsync(Email, Password);
            if (isSuccess)
            {
                // Navigate to Main App Services Page
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Login Failed. Please check your credentials.", "OK");
            }
        }

        [RelayCommand]
        private async Task GoToRegistration()
        {
            await Shell.Current.GoToAsync(nameof(RegistrationPage));
        }
    }
}